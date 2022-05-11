using System.Threading.Channels;

namespace PhotoOrganizings;

public class PhotoOrganizer
{
    public PhotoOrganizer(PhotoOrganizerOptions options) => Options = options;

    public event EventHandler<PhotoTask>? NewPhotoTaskEvent;

    public PhotoOrganizerOptions Options { get; }

    protected CancellationTokenSource? CancellationTokenSource { get; set; }

    private Channel<PhotoTask> PhotoTaskChanel { get; set; } = Channel.CreateUnbounded<PhotoTask>();

    public async Task StartAsync()
    {
        CancellationTokenSource = new CancellationTokenSource();

        try
        {
            await OrganizePhotos();
        }
        catch (OperationCanceledException)
        {
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
        PhotoTaskChanel.Writer.Complete();
    }

    protected Task OrganizePhotos()
    {
        return Task.Run(async () =>
        {
            foreach (FileInfo fileInfo in LoadFilesAsync(Options.InputFolderPath, Options.TargetFileTypes))
            {
                PhotoTask photoTask = new(fileInfo);
                await PhotoTaskChanel.Writer.WriteAsync(photoTask);
                OnNewPhotoTask(photoTask);
            }
        });
    }

    protected IEnumerable<FileInfo> LoadFilesAsync(string inputFolderPath, List<string> targetExtensions)
    {
        DirectoryInfo directoryInfo = new(inputFolderPath);
        IEnumerable<FileInfo> files = directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Where(s => Options.TargetFileTypes.Contains(s.Extension) is true);

        foreach (FileInfo fileInfo in files)
        {
            yield return fileInfo;
        }
    }

    protected virtual void OnNewPhotoTask(PhotoTask newPhotoTask) => NewPhotoTaskEvent?.Invoke(this, newPhotoTask);
}