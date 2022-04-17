using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
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
    private int _foundFilesCount;

    [ObservableProperty]
    private bool _hasPhotos;

    [ObservableProperty]
    private StorageFolder? _inputFolder;
    
    [ObservableProperty]
    private int _loadedFilesCount;

    [ObservableProperty]
    private StorageFolder? _outputFolder;

    [ObservableProperty]
    private string _outputFolderFormat = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PhotoViewModel> _photos = new();

    public MainWindowViewModel()
    {
        Logger = Log.ForContext<MainWindowViewModel>();
        Logger.Information("Initialized.");
    }

    private ILogger Logger { get; }

    [ICommand]
    private async Task LoadPhotosAsync(string? inputFolderPath, CancellationToken cancellationToken)
    {
        Logger.Verbose("LoadPhotosAsync [Folder:{Folder}]", inputFolderPath);

        if (LoadPhotosCommand.IsRunning || inputFolderPath is null)
        {
            Logger.Error("LoadPhotosAsync failed. [Folder:{Folder}][Running:{Running}]", inputFolderPath, LoadPhotosCommand.IsRunning);
            return;
        }

        StorageFolder? folder = await StorageFolder.GetFolderFromPathAsync(inputFolderPath);
        if (folder is null)
        {
            Logger.Error("LoadPhotosAsync failed. [Folder:{Folder}] is not valid.", inputFolderPath);
            return;
        }

        Photos.Clear();
        HasPhotos = false;

        List<string> fileTypeFilter = new();
        fileTypeFilter.Add(".jpg");
        fileTypeFilter.Add(".jpeg");
        QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);
        queryOptions.FolderDepth = FolderDepth.Deep;
        StorageFileQueryResult? results = folder.CreateFileQueryWithOptions(queryOptions);
        IReadOnlyList<StorageFile>? files = await results.GetFilesAsync();
        if (files is null)
        {
            Logger.Error("LoadPhotosAsync GetFilesAsync failed.");
            return;
        }

        IProgress<int> progress = new Progress<int>(x => LoadedFilesCount = x);
        FoundFilesCount = files.Count;
        int reportingInterval = Math.Max(files.Count / 100, 1);
        Photos = new ObservableCollection<PhotoViewModel>();

        foreach (StorageFile file in files)
        {
            Logger.Information("LoadPhotosAsync Loading [File:{File}]", file.Path);
            if (cancellationToken.IsCancellationRequested is true)
            {
                Logger.Warning("LoadPhotosAsync cancellation requested.");
                break;
            }

            string outputFolderPath = OutputFolder is not null ? OutputFolder.Path : string.Empty;

            PhotoViewModel photoViewModel = await new PhotoViewModelBuilder(file)
                .WithThumbnailService()
                .WithMetadata()
                .WithOutputFolderPath(outputFolderPath, OutputFolderFormat)
                .BuildAsync();

            Photos.Add(photoViewModel);

            if ((Photos.Count % reportingInterval) == 0)
            {
                progress.Report(Photos.Count);
            }

            Logger.Information("LoadPhotosAsync Loaded [File:{File}]", file.Path);
        }
        LoadedFilesCount = Photos.Count;
        
        HasPhotos = Photos.Count > 0;
    }

    [ICommand]
    private Task PreparePhotoAsync(int photoIndex)
    {
        Logger.Verbose("PreparePhotoAsync [index:{PhotoIndex}]", photoIndex);
        PhotoViewModel photoViewModel = Photos[photoIndex];
        return photoViewModel.LoadThumbnailAsync();
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
    private void UpdateOutputFolderFormat(string folderFormat) => OutputFolderFormat = folderFormat;

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
