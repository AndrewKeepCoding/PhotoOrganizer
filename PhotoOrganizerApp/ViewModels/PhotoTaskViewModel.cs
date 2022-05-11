using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace PhotoOrganizings.ViewModels;

[ObservableObject]
public partial class PhotoTaskViewModel
{
    private readonly PhotoTask _photoTask;

    [ObservableProperty]
    private BitmapImage? _thumbnail;

    [ObservableProperty]
    private string _inputFileName = string.Empty;

    [ObservableProperty]
    private string _inputFilePath = string.Empty;

    [ObservableProperty]
    private string? _fileSize;

    [ObservableProperty]
    private DateTime? _dateTaken;

    [ObservableProperty]
    private string? _outputFilePath;

    public PhotoTaskViewModel(PhotoTask photoTask)
    {
        _photoTask = photoTask;
        InputFileName = _photoTask.InputFileName;
        InputFilePath = _photoTask.InputFilePath;
    }
}