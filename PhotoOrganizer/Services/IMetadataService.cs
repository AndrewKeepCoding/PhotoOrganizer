using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace PhotoOrganizer.Services;

public interface IMetadataService
{
    Task<string?> GetHumanizedFileSize(StorageFile file);

    DateTime? GetTakenDate(StorageFile file);
}
