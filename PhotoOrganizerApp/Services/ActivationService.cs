using Microsoft.UI.Xaml;
using PhotoOrganizings.Activation;
using PhotoOrganizings.Interfaces;
using PhotoOrganizings.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoOrganizings.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;

    public ActivationService(
        ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
        IEnumerable<IActivationHandler> activationHandlers,
        IThemeSelectorService themeSelectorService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Initialize services that you need before app activation
        // take into account that the splash screen is shown while this code runs.
        await InitializeAsync();

        if (App.MainWindow is MainWindow mainWindow)
        {
            MainPage mainpage = App.GetService<MainPage>();
            mainWindow.SetBody(mainpage);
        }

        // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
        // will navigate to the first page
        await HandleActivationAsync(activationArgs);

        // Ensure the current window is active
        App.MainWindow.Activate();

        // Tasks after activation
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        IActivationHandler? activationHandler = _activationHandlers
            .FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler is not null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await _themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}