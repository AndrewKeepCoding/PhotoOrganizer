using CommunityToolkit.Diagnostics;
using PhotoOrganizings.Helpers;
using PhotoOrganizings.Interfaces;
using System.Threading.Tasks;
using Windows.Storage;

namespace PhotoOrganizings.Services;

public class LocalSettingsServicePackaged : ILocalSettingsService
{
    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out object? obj) is true &&
            obj is string jsonString)
        {
            return await JsonHelper.ToObjectAsync<T>(jsonString);
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        Guard.IsNotNull(value, nameof(value));
        ApplicationData.Current.LocalSettings.Values[key] = await JsonHelper.StringifyAsync(value);
    }
}