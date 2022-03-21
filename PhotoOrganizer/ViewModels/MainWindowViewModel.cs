using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotoOrganizer.Services;
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
    private readonly IThumbnailService _thumbnailService;
    [ObservableProperty]
    private StorageFolder? _inputFolder;

    [ObservableProperty]
    private bool _hasPhotos;

    [ObservableProperty]
    private int _foundFilesCount;

    [ObservableProperty]
    private int _loadedFilesCount;

    [ObservableProperty]
    private StorageFolder? _outputFolder;

    [ObservableProperty]
    private string _outputFolderFormat = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PhotoViewModel> _photos = new();

    public MainWindowViewModel(IThumbnailService thumbnailService)
    {
        _thumbnailService = thumbnailService;
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
        IProgress<int> progress = new Progress<int>(x => LoadedFilesCount = x);
        FoundFilesCount = files.Count;
        int reportingInterval = Math.Max(files.Count / 100, 1);

        foreach (StorageFile file in files)
        {
            if (cancellationToken.IsCancellationRequested is true)
                break;

            string outputFolderPath = OutputFolder is not null ? OutputFolder.Path : string.Empty;

            PhotoViewModel photoViewModel = await new PhotoViewModelBuilder(file)
                .WithThumbnailService()
                .WithMetadata()
                .WithOutputFolderPath(outputFolderPath, OutputFolderFormat)
                .BuildAsync();

            photoViewModels.Add(photoViewModel);

            if ((photoViewModels.Count % reportingInterval) == 0)
            {
                progress.Report(photoViewModels.Count);
            }
        }
        LoadedFilesCount = photoViewModels.Count;
        Photos = new ObservableCollection<PhotoViewModel>(photoViewModels);
        HasPhotos = Photos.Count > 0;
    }

    [ICommand]
    private Task PreparePhotoAsync(int photoIndex)
    {
        PhotoViewModel photoViewModel = Photos[photoIndex];
        return photoViewModel.LoadThumbnailAsync();
    }

    [ICommand]
    private void UpdateOutputFolderFormat(string folderFormat) => OutputFolderFormat = folderFormat;

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
