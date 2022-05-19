namespace PhotoOrganizings;

public enum PhotoTaskResult
{
    Running,
    Successed,
    Error,
}

public class PhotoTask
{
    public PhotoTask(ulong id, FileInfo fileInfo)
    {
        ID = id;
        InputFileInfo = fileInfo;
        Status = PhotoTaskResult.Running;
    }

    public ulong ID { get; }

    public FileInfo InputFileInfo { get; }

    public string InputFileFolderPath { get => InputFileInfo.DirectoryName ?? string.Empty; }

    public string InputFileName { get => InputFileInfo.Name; }

    public string InputFilePath { get => InputFileInfo.FullName; }

    public long FileSizeInBytes { get => InputFileInfo.Length; }

    public DateTime? DateTaken { get; set; }

    public FileInfo? OutputFileInfo { get; set; }

    public string OutputFileFolderPath { get => OutputFileInfo?.DirectoryName ?? string.Empty; }

    public string OutputFileName { get => OutputFileInfo?.Name ?? string.Empty; }

    public string OutputFilePath { get => OutputFileInfo?.FullName ?? string.Empty; }

    public PhotoTaskResult Status { get; set; }

    public Exception? Exception { get; set; }
}