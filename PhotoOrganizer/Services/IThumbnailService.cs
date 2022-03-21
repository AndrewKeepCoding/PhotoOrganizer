using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage;

namespace PhotoOrganizer.Services;

public interface IThumbnailService
{
    Task<BitmapImage?> GetThumbnail(StorageFile file);
}
