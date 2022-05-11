using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml.Media.Imaging;
using PhotoOrganizings.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace PhotoOrganizings.Services;

public class ThumbnailService : IThumbnailService
{
    private readonly IMemoryCache _memoryCache;
    private readonly SemaphoreSlim _semaphore = new(1);

    public ThumbnailService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public async Task<BitmapImage?> GetThumbnail(string filePath)
    {
        BitmapImage? thumbnail = null;
        await _semaphore.WaitAsync();

        try
        {
            StorageFile? storageFile = await StorageFile.GetFileFromPathAsync(filePath);

            if (_memoryCache.TryGetValue(storageFile.Path, out thumbnail) is true)
            {
                return thumbnail;
            }

            StorageItemThumbnail source = await storageFile.GetThumbnailAsync(ThumbnailMode.PicturesView);
            thumbnail = new();
            await thumbnail.SetSourceAsync(source);
            _ = _memoryCache.Set(storageFile.Path, thumbnail);
        }
        finally
        {
            _ = _semaphore.Release();
        }

        return thumbnail;
    }
}