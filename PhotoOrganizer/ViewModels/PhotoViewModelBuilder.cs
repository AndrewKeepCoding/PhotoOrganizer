using CommunityToolkit.Mvvm.DependencyInjection;
using PhotoOrganizer.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PhotoOrganizer.ViewModels;

public class PhotoViewModelBuilder
{
    private readonly StorageFile _file;
    private IMetadataService? _metadataService;
    private IThumbnailService? _thumbnailService;
    private string? _outputBaseFolderPath;
    private string? _outputFolderFormat;

    public PhotoViewModelBuilder(StorageFile file)
    {
        _file = file;
    }

    public async Task<PhotoViewModel> BuildAsync()
    {
        PhotoViewModel photoViewModel = new(_file, _thumbnailService);
        if (_metadataService is not null)
        {
            photoViewModel.DateTaken = _metadataService.GetTakenDate(_file);
            photoViewModel.FileSize = await _metadataService.GetHumanizedFileSize(_file);
        }
        if (_outputBaseFolderPath is not null &&
            _outputFolderFormat is not null &&
            photoViewModel.DateTaken is not null)
        {
            string? outputFilePath = CreateDateTimeFormatedFolderPath(
                photoViewModel.DateTaken,
                _outputBaseFolderPath,
                _outputFolderFormat);

            if (outputFilePath is not null)
                outputFilePath += $"\\{photoViewModel.InputFileName}";

            photoViewModel.OutputFilePath = outputFilePath;
        }
        return photoViewModel;
    }

    private static string? CreateDateTimeFormatedFolderPath(DateTime? dateTaken, string outputBaseFolderPath, string outputFolderFormat)
    {
        StringBuilder stringBuilder = new();
        return stringBuilder
            .Append(outputBaseFolderPath)
            .Append(dateTaken?.ToString(outputFolderFormat))
            .ToString();
    }

    public PhotoViewModelBuilder WithMetadata()
    {
        _metadataService = Ioc.Default.GetService<IMetadataService>();
        return this;
    }

    public PhotoViewModelBuilder WithOutputFolderPath(string outputBaseFolder, string outputFolderFormat)
    {
        _outputBaseFolderPath = outputBaseFolder;
        _outputFolderFormat = outputFolderFormat;
        return this;
    }

    public PhotoViewModelBuilder WithThumbnailService()
    {
        _thumbnailService = Ioc.Default.GetService<IThumbnailService>();
        return this;
    }
}
