using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizings.Interfaces;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizings.ViewModels;

[ObservableObject]
public partial class MainViewModel
{
    private readonly IPhotoOrganizerFactory _photoOrganizerFactory;
    private readonly IThumbnailService _thumbnailService;

    private readonly ObservableCollection<PhotoTaskViewModel> _photosObservableCollection = new();
    private readonly ConcurrentDictionary<ulong, PhotoTaskViewModel> _uncompletedPhotoTasks = new();

    [ObservableProperty]
    private List<string> _targetFileTypes = new() { ".jpg", ".jpeg", ".bmp", };

    [ObservableProperty]
    private string _inputFolderPath = @"C:\Photos\Input";// string.Empty;

    [ObservableProperty]
    private string _outputFolderPath = @"C:\Photos\Output";// string.Empty;

    [ObservableProperty]
    private bool _isSimulationMode = true;

    [ObservableProperty]
    private string _outputStructureFormat = string.Empty;

    [ObservableProperty]
    private bool _isOrganizingPhotos = false;

    [ObservableProperty]
    private OutputRootFolderNodeViewModel _outputRootFolderNode = new("");

    public MainViewModel(
        IPhotoOrganizerFactory photoOrganizerFactory,
        IThumbnailService thumbnailService)
    {
        _photoOrganizerFactory = photoOrganizerFactory;
        _thumbnailService = thumbnailService;
        PhotosCollectionView = new(_photosObservableCollection);
        DispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    public PhotoOrganizer? PhotoOrganizer { get; private set; }

    public AdvancedCollectionView PhotosCollectionView { get; private set; }
    private DispatcherQueue DispatcherQueue { get; }

    [ICommand]
    private async Task StartOrganizing()
    {
        Log.Logger.Information("StartOrganizing");
        IsOrganizingPhotos = true;

        PhotoOrganizerOptions options = new()
        {
            InputFolderPath = InputFolderPath,
            OutputFolderPath = OutputFolderPath,
            IsSimulationMode = IsSimulationMode,
            TargetFileTypes = TargetFileTypes,
            OutputStructureFormat = OutputStructureFormat.Replace(@"\", @"\\"),
        };

        _photosObservableCollection.Clear();
        PhotoOrganizer = _photoOrganizerFactory.Create(options);
        PhotoOrganizer.PhotoTaskCreated += PhotoOrganizer_PhotoTaskCreated;
        PhotoOrganizer.PhotoTaskCompleted += PhotoOrganizer_PhotoTaskCompleted;
        PhotoOrganizer.PhotoOrganizingStarted += PhotoOrganizer_PhotoOrganizingStarted;
        PhotoOrganizer.PhotoOrganizingCompleted += PhotoOrganizer_PhotoOrganizingCompleted;
        PhotoOrganizer.PhotoOrganizingCanceled += PhotoOrganizer_PhotoOrganizingCanceled;

        OutputRootFolderNode = new(OutputFolderPath);

        await PhotoOrganizer.StartAsync();

        IsOrganizingPhotos = false;
    }

    [ICommand]
    private void CancelOrganizing()
    {
        PhotoOrganizer?.Cancel();
    }

    [ICommand]
    private async void ItemsRepeaterElementPreparedEvent(ItemsRepeaterElementPreparedEventArgs? eventArgs)
    {
        int photoTaskIndex = eventArgs?.Index ?? -1;

        if (photoTaskIndex >= 0 && photoTaskIndex < _photosObservableCollection.Count)
        {
            PhotoTaskViewModel photoTaskViewModel = _photosObservableCollection[photoTaskIndex];
            photoTaskViewModel.Thumbnail ??= await _thumbnailService.GetThumbnail(photoTaskViewModel.InputFilePath);
        }
    }

    private void PhotoOrganizer_PhotoTaskCreated(object? sender, PhotoTask photoTask)
    {
        Log.Logger.Information($"PhotoTaskCreated PhotoTask [{photoTask.ID}: {photoTask.InputFileName}] Start");
        PhotoTaskViewModel photoTaskViewModel = new(photoTask);
        _uncompletedPhotoTasks[photoTaskViewModel.PhotoTask.ID] = photoTaskViewModel;

        if (DispatcherQueue.TryEnqueue(() => _photosObservableCollection.Add(photoTaskViewModel)) is false)
        {
            Log.Logger.Error($"PhotoTaskCreated TryEnqueue() failed trying PhotoTask [{photoTask.ID}: {photoTask.InputFileName}]");
        }

        Log.Logger.Information($"PhotoTaskCreated PhotoTask [{photoTask.ID}: {photoTask.InputFileName}] End");
    }

    private void PhotoOrganizer_PhotoTaskCompleted(object? sender, PhotoTask photoTask)
    {
        Log.Logger.Information($"PhotoTaskCompleted PhotoTask [{photoTask.ID}: {photoTask.InputFileName}] Start");
        if (_uncompletedPhotoTasks.TryGetValue(photoTask.ID, out PhotoTaskViewModel? photoTaskViewModel) is true)
        {
            if (DispatcherQueue.TryEnqueue(() =>
            {
                photoTaskViewModel.OutputFilePath = photoTask.OutputFilePath;
                photoTaskViewModel.Status = photoTask.Status;

                OutputFileNodeViewModel fileNode = new(photoTask.OutputFilePath);
                OutputRootFolderNode.AddFileChild(fileNode);
            }) is false)
            {
                Log.Logger.Error($"PhotoTaskCompleted TryEnqueue() failed trying PhotoTask [{photoTask.ID}: {photoTask.InputFileName}]");
            }
        }
        Log.Logger.Information($"PhotoTaskCompleted PhotoTask [{photoTask.ID}: {photoTask.InputFileName}] End");
    }

    private void PhotoOrganizer_PhotoOrganizingStarted(object? sender, EventArgs e)
    {
        Log.Logger.Information("PhotoOrganizer started event");
    }

    private void PhotoOrganizer_PhotoOrganizingCompleted(object? sender, EventArgs e)
    {
        Log.Logger.Information("PhotoOrganizer completed event");
    }

    private void PhotoOrganizer_PhotoOrganizingCanceled(object? sender, EventArgs e)
    {
        Log.Logger.Information("PhotoOrganizer cancel event");
    }
}