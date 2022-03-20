using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace PhotoOrganizer.ViewModels;

[ObservableObject]
public partial class MainWindowViewModel
{
    [ObservableProperty]
    private StorageFolder? _inputFolder;

    [ObservableProperty]
    private StorageFolder? _outputFolder;

    public MainWindowViewModel()
    {

    }

    [ICommand]
    private async Task UpdateInputFolderPath(string? folderPath)
    {
        if (folderPath is null)
            return;

        StorageFolder? folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
        if (folder is null)
            return;

        InputFolder = folder;
    }

    [ICommand]
    private async Task UpdateOutputFolderPath(string? folderPath)
    {
        if (folderPath is null)
            return;

        StorageFolder? folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
        if (folder is null)
            return;

        OutputFolder = folder;
    }
}
