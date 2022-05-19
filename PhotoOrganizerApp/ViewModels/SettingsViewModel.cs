using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using PhotoOrganizings.Helpers;
using PhotoOrganizings.Interfaces;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace PhotoOrganizings.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    [ObservableProperty]
    private ElementTheme _theme;

    [ObservableProperty]
    private string _versionDescription = string.Empty;

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        Theme = _themeSelectorService.Theme;
        VersionDescription = GetVersionDescription();
    }

    private static string GetVersionDescription()
    {
        string appName = "AppDisplayName".GetLocalized();
        PackageVersion version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    [ICommand]
    private async Task SwitchTheme(ElementTheme theme)
    {
        if (Theme != theme)
        {
            Theme = theme;
            await _themeSelectorService.SetThemeAsync(Theme);
        }
    }
}