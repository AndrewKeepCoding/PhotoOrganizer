using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Humanizer.Bytes;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace PhotoOrganizings.ViewModels;

[ObservableObject]
public partial class PhotoTaskViewModel
{
    public PhotoTask PhotoTask { get; }

    public string ID { get; } = string.Empty;

    public string InputFileName { get; } = string.Empty;

    public string InputFilePath { get; } = string.Empty;

    [ObservableProperty]
    private BitmapImage? _thumbnail;

    [ObservableProperty]
    private string? _fileSize;

    [ObservableProperty]
    private DateTime? _dateTaken;

    [ObservableProperty]
    private string? _outputFilePath;

    [ObservableProperty]
    private PhotoTaskResult _status;

    public PhotoTaskViewModel(PhotoTask photoTask)
    {
        PhotoTask = photoTask;
        ID = PhotoTask.ID.ToString();
        InputFileName = PhotoTask.InputFileName;
        InputFilePath = PhotoTask.InputFilePath;
        //OutputFilePath = PhotoTask.OutputFilePath;
        DateTaken = PhotoTask.DateTaken;
        FileSize = new ByteSize(PhotoTask.FileSizeInBytes).Humanize();
        Status = photoTask.Status;
    }
}