using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using PhotoOrganizer.Services;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace PhotoOrganizer.ViewModels;

[ObservableObject]
public partial class PhotoViewModel
{
    private readonly StorageFile _file;
    private readonly IThumbnailService _thumbnailService;
    [ObservableProperty]
    private BitmapImage? _thumbnail;

    [ObservableProperty]
    private string? _inputFileName;

    [ObservableProperty]
    private string? _inputFilePath;

    [ObservableProperty]
    private string? _fileSize;

    [ObservableProperty]
    private DateTime? _dateTaken;

    [ObservableProperty]
    private string? _outputFilePath;

    public PhotoViewModel(StorageFile file, IThumbnailService thumbnailService)
    {
        _file = file;
        _thumbnailService = thumbnailService;
        InputFileName = _file.Name;
        InputFilePath = _file.Path.ToString();
    }

    public async Task LoadThumbnailAsync()
    {
        if (Thumbnail is null)
        {
            Thumbnail = await _thumbnailService.GetThumbnail(_file);
        }
    }
}
