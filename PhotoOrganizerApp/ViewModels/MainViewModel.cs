using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizings.Interfaces;
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

    [ObservableProperty]
    private OutputRootFolderNodeViewModel? _outputRootFolderNode;

    private DispatcherQueue DispatcherQueue { get; }

    [ICommand]
    private async Task StartOrganizing()
    {
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
        PhotoOrganizer.PhotoTaskCreated += PhotoOrganizer_NewPhotoTaskEvent;
        PhotoOrganizer.PhotoTaskCompleted += PhotoOrganizer_PhotoTaskCompleted;

        OutputRootFolderNode = new(OutputFolderPath);

        await PhotoOrganizer.StartAsync();
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

    private void PhotoOrganizer_NewPhotoTaskEvent(object? sender, PhotoTask photoTask)
    {
        PhotoTaskViewModel photoTaskViewModel = new(photoTask);
        _uncompletedPhotoTasks[photoTaskViewModel.PhotoTask.ID] = photoTaskViewModel;
        bool successed = DispatcherQueue.TryEnqueue(() => _photosObservableCollection.Add(photoTaskViewModel));
        if (successed is false)
        {
            bool stop = true;
        }
    }

    private void PhotoOrganizer_PhotoTaskCompleted(object? sender, PhotoTask photoTask)
    {
        if (_uncompletedPhotoTasks.TryGetValue(photoTask.ID, out PhotoTaskViewModel? photoTaskViewModel) is true)
        {
            bool successed = DispatcherQueue.TryEnqueue(() =>
            {
                photoTaskViewModel.OutputFilePath = photoTask.OutputFilePath;
                photoTaskViewModel.Status = photoTask.Status;

                OutputFileNodeViewModel fileNode = new(photoTask.OutputFilePath);
                OutputRootFolderNode?.AddFileChild(fileNode);
            });
            if (successed is false)
            {
                bool stop = true;
            }
        }
    }
}