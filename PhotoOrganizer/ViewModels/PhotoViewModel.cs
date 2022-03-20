using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage;

namespace PhotoOrganizer.ViewModels;

[ObservableObject]
public partial class PhotoViewModel
{
    private readonly StorageFile _file;

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

    public PhotoViewModel(StorageFile file)
    {
        _file = file;

        InputFileName = _file.Name;
        InputFilePath = _file.Path.ToString();
    }
}
