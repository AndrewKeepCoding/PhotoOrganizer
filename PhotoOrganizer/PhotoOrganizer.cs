using PhotoOrganizings.Services;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace PhotoOrganizings;

public class PhotoOrganizer
{
    public PhotoOrganizer(PhotoOrganizerOptions options) => Options = options;

    public event EventHandler<PhotoTask>? PhotoTaskCreated;

    public event EventHandler<PhotoTask>? PhotoTaskCompleted;

    public event EventHandler? PhotoOrganizingStarted;

    public event EventHandler? PhotoOrganizingCompleted;

    public event EventHandler? PhotoOrganizingCanceled;

    public PhotoOrganizerOptions Options { get; }

    protected CancellationTokenSource? CancellationTokenSource { get; set; }

    private Channel<PhotoTask> PhotoTaskChannel { get; set; } = Channel.CreateUnbounded<PhotoTask>();

    public async Task StartAsync()
    {
        ValidateOptions();
        CancellationTokenSource = new CancellationTokenSource();

        try
        {
            OnPhotoOrganizingStarted();
            await OrganizePhotos(CancellationTokenSource.Token);
            OnPhotoOrganizingCompleted();
        }
        catch (OperationCanceledException)
        {
            OnPhotoOrganizingCanceled();
        }
        catch (Exception)
        {
            // TODO: Throw custom exceptions
            throw;
        }
        finally
        {
            CancellationTokenSource temp = CancellationTokenSource;
            CancellationTokenSource = null;
            temp?.Dispose();
        }
    }

    public void Cancel()
    {
        CancellationTokenSource?.Cancel();
        PhotoTaskChannel.Writer.Complete();
    }

    protected void ValidateOptions()
    {
        if (Directory.Exists(Options.InputFolderPath) is false)
        {
            throw new Exception($"InputFolderPath: {Options.InputFolderPath} does not exits.");
        }

        if (Directory.Exists(Options.OutputFolderPath) is false)
        {
            throw new Exception($"OutputFolderPath: {Options.OutputFolderPath} does not exits.");
        }
    }

    protected static string? CreateDateTimeFormatedFolderPath(DateTime? dateTaken, string outputBaseFolderPath, string outputFolderFormat)
    {
        StringBuilder stringBuilder = new();
        return stringBuilder
            .Append(outputBaseFolderPath)
            .Append(dateTaken?.ToString(outputFolderFormat))
            .ToString();
    }

    protected Task OrganizePhotos(CancellationToken cancellationToken)
    {
        Task inputTask = Task.Run(() => CreatePhotoTasks(cancellationToken), CancellationToken.None);
        Task outputTask = Task.Run(() => ProcessPhotoTasks(cancellationToken), CancellationToken.None);

        return Task.WhenAll(inputTask, outputTask);
    }

    protected async Task CreatePhotoTasks(CancellationToken cancellationToken)
    {
        ulong photoTaskId = 1;

        foreach (FileInfo fileInfo in LoadFiles(Options.InputFolderPath, Options.TargetFileTypes))
        {
            cancellationToken.ThrowIfCancellationRequested();

            PhotoTask photoTask = CreatePhotoTask(photoTaskId, fileInfo);
            photoTaskId++;
            OnPhotoTaskCreated(photoTask);
            await PhotoTaskChannel
                .Writer
                .WriteAsync(photoTask, cancellationToken)
                .ConfigureAwait(false);
        }

        PhotoTaskChannel.Writer.Complete();
    }

    protected async Task ProcessPhotoTasks(CancellationToken cancellationToken)
    {
        await foreach (PhotoTask photoTask in PhotoTaskChannel.Reader
            .ReadAllAsync(cancellationToken)
            .ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Remove this when the TreeView bug gets fixed.
            await Task.Delay(10, cancellationToken);

            try
            {
                string outputFileFolderPath = CreateDateTimeFormatedFolderPath(
                    photoTask.DateTaken,
                    Options.OutputFolderPath,
                    Options.OutputStructureFormat) ?? Options.OutputFolderPath;

                string outputFilePath = @$"{outputFileFolderPath}\{photoTask.InputFileName}";

                outputFilePath = AddIndexToOutputFileNameIfNecessary(outputFilePath);

                if (File.Exists(outputFilePath) is false && Options.IsSimulationMode is false)
                {
                    CreateFolder(outputFileFolderPath);
                    photoTask.OutputFileInfo = photoTask.InputFileInfo.CopyTo(outputFilePath);
                }
                else
                {
                    photoTask.OutputFileInfo = new FileInfo(outputFilePath);
                }
            }
            catch (Exception exception)
            {
                photoTask.Status = PhotoTaskResult.Error;
                photoTask.Exception = exception;
            }

            if (photoTask.Status is not PhotoTaskResult.Error)
            {
                photoTask.Status = PhotoTaskResult.Successed;
            }

            OnPhotoTaskCompleted(photoTask);
        }
    }

    protected IEnumerable<FileInfo> LoadFiles(string inputFolderPath, List<string> targetExtensions)
    {
        DirectoryInfo directoryInfo = new(inputFolderPath);
        IEnumerable<FileInfo> files = directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Where(s => Options.TargetFileTypes.Contains(s.Extension) is true);

        foreach (FileInfo fileInfo in files)
        {
            yield return fileInfo;
        }
    }

    protected PhotoTask CreatePhotoTask(ulong id, FileInfo fileInfo)
    {
        DateTime? dateTaken = MetadataService.GetTakenDate(fileInfo.FullName);

        return new(id, fileInfo)
        {
            DateTaken = dateTaken,
        };
    }

    protected string AddIndexToOutputFileNameIfNecessary(string prefferedOutputFilePath)
    {
        if (File.Exists(prefferedOutputFilePath) is false)
        {
            return prefferedOutputFilePath;
        }

        string fileName = Path.GetFileNameWithoutExtension(prefferedOutputFilePath);
        string? folderPath = Path.GetDirectoryName(prefferedOutputFilePath);

        if (folderPath is null)
        {
            throw new ArgumentException($"{prefferedOutputFilePath} has no valid folder path.");
        }

        string fileExtension = Path.GetExtension(prefferedOutputFilePath);

        int index = 1;
        Match match = Regex.Match(fileName, @"^(.+) \((\d+)\)$");

        if (match.Success is true)
        {
            fileName = match.Groups[1].Value;
            index = int.Parse(match.Groups[2].Value);
        }

        string outputFilePath;

        do
        {
            index++;
            string newFileName = $"{fileName} ({index}){fileExtension}";
            outputFilePath = Path.Combine(folderPath, newFileName);
        }
        while (File.Exists(outputFilePath) is true);

        return outputFilePath;
    }

    protected static void CreateFolder(string folderPath)
    {
        DirectoryInfo directoryInfo = new(folderPath);

        if (directoryInfo.Exists is false)
        {
            directoryInfo.Create();
        }
    }

    protected virtual void OnPhotoOrganizingStarted() => PhotoOrganizingStarted?.Invoke(this, EventArgs.Empty);

    protected virtual void OnPhotoOrganizingCompleted() => PhotoOrganizingCompleted?.Invoke(this, EventArgs.Empty);

    protected virtual void OnPhotoOrganizingCanceled() => PhotoOrganizingCanceled?.Invoke(this, EventArgs.Empty);

    protected virtual void OnPhotoTaskCreated(PhotoTask photoTask) => PhotoTaskCreated?.Invoke(this, photoTask);

    protected virtual void OnPhotoTaskCompleted(PhotoTask photoTask) => PhotoTaskCompleted?.Invoke(this, photoTask);
}