using Microsoft.UI.Xaml;
using PhotoOrganizings.Interfaces;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizings.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    private const string SettingsKey = "AppBackgroundRequestedTheme";

    private readonly ILocalSettingsService _localSettingsService;

    public ThemeSelectorService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public ElementTheme Theme { get; private set; } = ElementTheme.Default;

    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;
        }

        await Task.CompletedTask;
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        ElementTheme cacheTheme = ElementTheme.Default;

        if (await _localSettingsService.ReadSettingAsync<string>(SettingsKey) is string themeName &&
            themeName.Length > 0)
        {
            _ = Enum.TryParse(themeName, out cacheTheme);
        }

        return cacheTheme;
    }

    private async Task SaveThemeInSettingsAsync(ElementTheme theme)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey, theme.ToString());
    }
}