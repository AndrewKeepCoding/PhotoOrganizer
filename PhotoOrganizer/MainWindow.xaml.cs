using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizer.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PhotoOrganizer;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        Title = "Photo Organizer";
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);

        ViewModel = Ioc.Default.GetService<MainWindowViewModel>();

        UpdateOutputFolderExample();
    }

    public StorageFolder? SelectedInputFolder { get; set; }
    public StorageFolder? SelectedOutputFolder { get; set; }
    public MainWindowViewModel? ViewModel { get; }

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


            ViewModel.LoadPhotosCommand?.ExecuteAsync(SelectedInputFolder?.Path);
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

    private void PhotosList_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        ViewModel?.PreparePhotoCommand?.ExecuteAsync(args.Index);
    }
}
