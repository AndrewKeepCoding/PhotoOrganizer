namespace PhotoOrganizings;

public class PhotoTask
{
    public PhotoTask(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    public FileInfo FileInfo { get; }

    public string InputFileName { get => FileInfo.Name; }

    public string InputFilePath { get => FileInfo.FullName; }

    public string OutputFilePath { get; set; } = string.Empty;

    public string OutputFileName { get; set; } = string.Empty;

    public bool IsCompleted { get; set; } = false;
}