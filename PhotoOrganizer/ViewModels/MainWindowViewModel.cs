using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

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
    private async Task LoadPhotosAsync(string? inputFolderPath, CancellationToken cancellationToken)
    {
        if (LoadPhotosCommand.IsRunning || inputFolderPath is null)
            return;

        StorageFolder? folder = await StorageFolder.GetFolderFromPathAsync(inputFolderPath);
        if (folder is null)
            return;

        List<string> fileTypeFilter = new();
        fileTypeFilter.Add(".jpg");
        fileTypeFilter.Add(".jpeg");
        QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);
        queryOptions.FolderDepth = FolderDepth.Deep;
        StorageFileQueryResult? results = folder.CreateFileQueryWithOptions(queryOptions);
        IReadOnlyList<StorageFile>? files = await results.GetFilesAsync();
        if (files is null)
            return;


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
