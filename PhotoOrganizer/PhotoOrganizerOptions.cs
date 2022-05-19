namespace PhotoOrganizings;

public record PhotoOrganizerOptions
{
    public string InputFolderPath { get; init; } = string.Empty;
    public string OutputFolderPath { get; init; } = string.Empty;
    public bool IsSimulationMode { get; init; } = false;
    public List<string> TargetFileTypes { get; init; } = new() { "jpg", "jpeg", "bmp", };
    public string OutputStructureFormat { get; init; } = string.Empty;
}