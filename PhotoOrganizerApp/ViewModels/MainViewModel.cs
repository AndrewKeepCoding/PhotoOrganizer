using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizings.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizings.ViewModels;

[ObservableObject]
public partial class MainViewModel
{
    private readonly IPhotoOrganizerFactory _photoOrganizerFactory;
    private readonly IThumbnailService _thumbnailService;
    private readonly IMetadataService _metadataService;

    private readonly ObservableCollection<PhotoTaskViewModel> _photosObservableCollection = new();

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
        IThumbnailService thumbnailService,
        IMetadataService metadataService)
    {
        _photoOrganizerFactory = photoOrganizerFactory;
        _thumbnailService = thumbnailService;
        _metadataService = metadataService;
        PhotosCollectionView = new(_photosObservableCollection);
        DispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    public PhotoOrganizer? PhotoOrganizer { get; private set; }

    public AdvancedCollectionView PhotosCollectionView { get; private set; }

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
            OutputStructureFormat = OutputStructureFormat,
        };
        _photosObservableCollection.Clear();
        PhotoOrganizer = _photoOrganizerFactory.Create(options);
        PhotoOrganizer.NewPhotoTaskEvent += PhotoOrganizer_NewPhotoTaskEvent;

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

            photoTaskViewModel.DateTaken = _metadataService.GetTakenDate(photoTaskViewModel.InputFilePath);
            photoTaskViewModel.FileSize = await _metadataService.GetHumanizedFileSize(photoTaskViewModel.InputFilePath);
        }
    }

    private void PhotoOrganizer_NewPhotoTaskEvent(object? sender, PhotoTask newPhotoTask)
    {
        _ = DispatcherQueue.TryEnqueue(() => _photosObservableCollection.Add(new PhotoTaskViewModel(newPhotoTask)));
    }
}