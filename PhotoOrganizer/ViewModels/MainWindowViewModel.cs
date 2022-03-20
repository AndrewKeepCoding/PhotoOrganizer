using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


    [ObservableProperty]
    private ObservableCollection<PhotoViewModel> _photos = new();

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

        Photos.Clear();


        List<string> fileTypeFilter = new();
        fileTypeFilter.Add(".jpg");
        fileTypeFilter.Add(".jpeg");
        QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);
        queryOptions.FolderDepth = FolderDepth.Deep;
        StorageFileQueryResult? results = folder.CreateFileQueryWithOptions(queryOptions);
        IReadOnlyList<StorageFile>? files = await results.GetFilesAsync();
        if (files is null)
            return;

        List<PhotoViewModel> photoViewModels = new();

        foreach (StorageFile file in files)
        {
            if (cancellationToken.IsCancellationRequested is true)
                break;

            PhotoViewModel photoViewModel = new(file);

            photoViewModels.Add(photoViewModel);


        }

        Photos = new ObservableCollection<PhotoViewModel>(photoViewModels);


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
