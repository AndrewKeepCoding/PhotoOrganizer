using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace PhotoOrganizer.Services;

public class ThumbnailService : IThumbnailService
{
    private readonly IMemoryCache _memoryCache;
    private SemaphoreSlim _semaphore = new(1);

    public ThumbnailService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<BitmapImage?> GetThumbnail(StorageFile file)
    {
        BitmapImage? thumbnail = null;
        await _semaphore.WaitAsync();
        try
        {
            if (_memoryCache.TryGetValue(file.Path, out thumbnail) is true)
            {
                return thumbnail;
            }

            StorageItemThumbnail source = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
            thumbnail = new();
            await thumbnail.SetSourceAsync(source);
            _memoryCache.Set(file.Path, thumbnail);
        }
        finally
        {
            _semaphore.Release();
        }

        return thumbnail;
    }
}
