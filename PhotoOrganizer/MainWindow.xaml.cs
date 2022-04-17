using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PhotoOrganizer.Utilities;
using PhotoOrganizer.ViewModels;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.WinUi3;
using Serilog.Sinks.WinUi3.LogViewModels;
using Serilog.Templates;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PhotoOrganizer;

public sealed partial class MainWindow : Window
{
    private ItemsRepeaterLogBroker? _logBroker;

    public MainWindow()
    {
        this.InitializeComponent();

        ConfigureLogger();
        Logger = Log.ForContext<MainWindow>();

        Title = "Photo Organizer";
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);

        ViewModel = Ioc.Default.GetService<MainWindowViewModel>();

        UpdateOutputFolderExample();

        Logger.Information("Initialized.");
    }

    public StorageFolder? SelectedInputFolder { get; set; }
    public StorageFolder? SelectedOutputFolder { get; set; }
    public MainWindowViewModel? ViewModel { get; }

    private ILogger Logger { get; }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.LoadPhotosCommand?.Cancel();
    }

    private void CloseInfoSecreenButton_Click(object sender, RoutedEventArgs e)
    {
        InfoScreen.Visibility = Visibility.Collapsed;
    }

    private void ConfigureLogger()
    {
        App.Current.Resources.TryGetValue("DefaultTextForegroundThemeBrush", out object defaultTextForegroundThemeBrush);

        _logBroker = new ItemsRepeaterLogBroker(
            LogItemsRepeater,
            LogScrollViewer,
            new EmojiLogViewModelBuilder((defaultTextForegroundThemeBrush as SolidColorBrush)?.Color)

                .SetTimestampFormat(new ExpressionTemplate("{@t:yyyy-MM-dd HH:mm:ss.fff}"))

                .SetLevelsFormat(new ExpressionTemplate("{@l:u3}"))
                .SetLevelForeground(LogEventLevel.Verbose, Colors.Gray)
                .SetLevelForeground(LogEventLevel.Debug, Colors.Gray)
                .SetLevelForeground(LogEventLevel.Information, Colors.White)
                .SetLevelForeground(LogEventLevel.Warning, Colors.Yellow)
                .SetLevelForeground(LogEventLevel.Error, Colors.Red)
                .SetLevelForeground(LogEventLevel.Fatal, Colors.HotPink)

                .SetSourceContextFormat(new ExpressionTemplate("{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}"))

                .SetMessageFormat(new ExpressionTemplate("{@m}"))
                .SetMessageForeground(LogEventLevel.Verbose, Colors.Gray)
                .SetMessageForeground(LogEventLevel.Debug, Colors.Gray)
                .SetMessageForeground(LogEventLevel.Information, Colors.White)
                .SetMessageForeground(LogEventLevel.Warning, Colors.Yellow)
                .SetMessageForeground(LogEventLevel.Error, Colors.Red)
                .SetMessageForeground(LogEventLevel.Fatal, Colors.HotPink)

                .SetExceptionFormat(new ExpressionTemplate("{@x}"))
                .SetExceptionForeground(Colors.HotPink)
            );

        Log.Logger = new LoggerConfiguration()
            .WriteTo.WinUi3Control(_logBroker)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _logBroker.IsAutoScrollOn = true;
    }

    private string CreateDateFolderFormat()
    {
        string format = string.Empty;

        if (CreateYearFolderCheckBox.IsChecked is true)
            format += @"\\yyyy";
        if (CreateMonthFolderCheckBox.IsChecked is true)
            format += @"\\MM";
        if (CreateDayFolderCheckBox.IsChecked is true)
            format += @"\\dd";
        if (CreateDateFolderCheckBox.IsChecked is true)
            format += @"\\yyyy-MM-dd";

        return format;
    }

    private void FolderStructureCheckBox_Click(object sender, RoutedEventArgs e) => UpdateOutputFolderExample();

    private void LogViewerButton_Click(object sender, RoutedEventArgs e)
    {
        InfoScreen.Visibility = (InfoScreen.Visibility is Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PhotosList_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        ViewModel?.PreparePhotoCommand?.ExecuteAsync(args.Index);
    }

    private async Task<StorageFolder?> SelectFolderAsync()
    {
        FolderPicker folderPicker = new();
        folderPicker.FileTypeFilter.Add("*");
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        return await folderPicker.PickSingleFolderAsync();
    }

    private async void SelectInputFolderButton_Click(object sender, RoutedEventArgs e)
    {
        StorageFolder? folder = await SelectFolderAsync();
        if (folder is not null)
        {
            SelectedInputFolder = folder;
            SelectedInputFolderTextBox.Text = SelectedInputFolder.Path;
        }
    }

    private async void SelectOutputFolderButton_Click(object sender, RoutedEventArgs e)
    {
        StorageFolder? folder = await SelectFolderAsync();
        if (folder is not null)
        {
            SelectedOutputFolder = folder;
            SelectedOutputFolderTextBox.Text = SelectedOutputFolder.Path;
        }
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        ContentDialogResult result = await StartSettingsDialog.ShowAsync();
        if (result is ContentDialogResult.Primary && ViewModel is not null)
        {
            ViewModel.UpdateInputFolderPathCommand?.Execute(SelectedInputFolder?.Path);
            ViewModel.UpdateOutputFolderPathCommand?.Execute(SelectedOutputFolder?.Path);

            string folderFormat = CreateDateFolderFormat();
            ViewModel.UpdateOutputFolderFormatCommand?.Execute(folderFormat);

            ProgessInfo.Visibility = Visibility.Visible;
            InfoScreen.Visibility = Visibility.Visible;

            await ViewModel.LoadPhotosCommand.ExecuteAsync(SelectedInputFolder?.Path);

            ProgessInfo.Visibility = Visibility.Collapsed;
            InfoScreen.Visibility = Visibility.Collapsed;
        }
    }

    private void UpdateOutputFolderExample()
    {
        string example = @"[Output]";

        if (SelectedOutputFolder?.Path.Length > 0)
            example = SelectedOutputFolder.Path;

        string dateFormat = CreateDateFolderFormat();
        if (dateFormat.Length > 0)
            example += DateTime.Now.ToString(dateFormat, CultureInfo.InvariantCulture);

        example += @"\[Filename]";

        ExampleTextBlock.Text = example;
    }
}
