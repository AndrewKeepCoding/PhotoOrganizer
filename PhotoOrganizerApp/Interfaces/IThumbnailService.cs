using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

namespace PhotoOrganizings.Interfaces;

public interface IThumbnailService
{
    Task<BitmapImage?> GetThumbnail(string filePath);
}