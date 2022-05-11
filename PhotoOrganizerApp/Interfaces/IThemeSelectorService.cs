using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace PhotoOrganizings.Interfaces;

public interface IThemeSelectorService
{
    ElementTheme Theme { get; }

    Task InitializeAsync();

    Task SetThemeAsync(ElementTheme theme);

    Task SetRequestedThemeAsync();
}