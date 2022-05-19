using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace PhotoOrganizings.Services;

public class MetadataService
{
    public static DateTime? GetTakenDate(string filePath)
    {
        IReadOnlyList<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(filePath);
        ExifSubIfdDirectory? directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

        return directory?.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out DateTime dateTime) is true ? dateTime : null;
    }
}