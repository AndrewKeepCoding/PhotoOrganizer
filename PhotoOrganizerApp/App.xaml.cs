using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using PhotoOrganizings.Activation;
using PhotoOrganizings.Factories;
using PhotoOrganizings.Helpers;
using PhotoOrganizings.Interfaces;
using PhotoOrganizings.Models;
using PhotoOrganizings.Services;
using PhotoOrganizings.ViewModels;
using PhotoOrganizings.Views;

namespace PhotoOrganizings;

public partial class App : Application
{
    public static Window MainWindow { get; } = new MainWindow()
    {
        Title = "AppDisplayName".GetLocalized()
    };

    private static readonly IHost _host = Host
    .CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // Default Activation Handler
        _ = services
        .AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>()
        // Extensions
        .AddMemoryCache()
        // Core
        .AddTransient<PhotoOrganizer>()
        // Factories
        .AddSingleton<IPhotoOrganizerFactory, PhotoOrganizerFactory>()
        // Services
        .AddSingleton<ILocalSettingsService, LocalSettingsServicePackaged>()
        .AddSingleton<IThemeSelectorService, ThemeSelectorService>()
        .AddSingleton<IActivationService, ActivationService>()
        .AddSingleton<IThumbnailService, ThumbnailService>()
        // Views and ViewModels
        .AddSingleton<LogsViewModel>()
        .AddSingleton<LogsPage>()
        .AddSingleton<SettingsViewModel>()
        .AddSingleton<SettingsPage>()
        .AddSingleton<MainViewModel>()
        .AddSingleton<MainPage>()
        // Configuration
        .Configure<PhotoOrganizerOptions>(context.Configuration.GetSection(nameof(PhotoOrganizerOptions)))
        .Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
    })
    .Build();

    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        IActivationService? activationService = App.GetService<IActivationService>();

        if (activationService is not null)
        {
            await activationService.ActivateAsync(args);
        }
    }

    public static T GetService<T>() where T : class
    {
        return _host.Services.GetService(typeof(T)) as T ??
            throw new System.ArgumentException($"Service {typeof(T)} not found. Did you forget to register the service?");
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}