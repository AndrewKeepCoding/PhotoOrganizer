using Humanizer;
using Humanizer.Bytes;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoOrganizings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace PhotoOrganizings.Services;

public class MetadataService : IMetadataService
{
    public async Task<string?> GetHumanizedFileSize(string filePath)
    {
        StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
        BasicProperties basicProperties = await storageFile.GetBasicPropertiesAsync();

        return new ByteSize(basicProperties.Size).Humanize();
    }

    public DateTime? GetTakenDate(string filePath)
    {
        IReadOnlyList<Directory> directories = ImageMetadataReader.ReadMetadata(filePath);
        ExifSubIfdDirectory? directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

        return directory?.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out DateTime dateTime) is true ? dateTime : null;
    }
}