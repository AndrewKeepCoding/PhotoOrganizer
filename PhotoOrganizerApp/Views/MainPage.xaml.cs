using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizings.Helpers;
using PhotoOrganizings.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PhotoOrganizings.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    public MainViewModel ViewModel { get; }

    private string CreateOutputStructureFormat()
    {
        string format = string.Empty;

        if (CustomOutputStructureCheckBox.IsChecked is true)
        {
            format = OutputStructureTextBox.Text;
        }
        else
        {
            if (YearFolderCheckBox.IsChecked is true)
                format += (format.Length > 0 ? @"\" : @"") + @"yyyy";
            if (MonthFolderCheckBox.IsChecked is true)
                format += (format.Length > 0 ? @"\" : @"") + @"MM";
            if (DayFolderCheckBox.IsChecked is true)
                format += (format.Length > 0 ? @"\" : @"") + @"dd";
            if (DateFolderCheckBox.IsChecked is true)
                format += (format.Length > 0 ? @"\" : @"") + @"yyyy-MM-dd";
        }

        if (format.Length > 0 && format.StartsWith(@"\") is false)
        {
            format = @"\" + format;
        }

        return format;
    }

    private void UpdateOutputStructure()
    {
        string sample = string.Empty;

        try
        {
            string format = CreateOutputStructureFormat();
            OutputStructureTextBox.Text = format;
            format = format.Replace(@"\", @"\\");
            sample = format.Length > 0 ? DateTime.Now.ToString(format) : string.Empty;
            OutputStructureErrorMessageTextBox.Visibility = Visibility.Collapsed;
        }
        catch (Exception)
        {
            OutputStructureErrorMessageTextBox.Visibility = Visibility.Visible;
        }

        OutputStructureSampleTextBox.Text = @$"[{"OutputFolder".GetLocalized()}]{sample}\[{"FileName".GetLocalized()}]";
    }

    private void OutputStructureCheckBox_Click(object sender, RoutedEventArgs e) => UpdateOutputStructure();

    private void DefaultOutputStructureButton_Click(object sender, RoutedEventArgs e) => OutputStructureTextBox.Text = @"yyyy\MM\dd\yyyy-MM-dd";

    private void SettingsButton_Click(object sender, RoutedEventArgs e) => SettingsPage.Visibility = Visibility.Visible;

    private void LogsButton_Click(object sender, RoutedEventArgs e) => LogsPage.Visibility = Visibility.Visible;

    private void OutputStructureTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateOutputStructure();

    private void Page_Loaded(object sender, RoutedEventArgs e) => UpdateOutputStructure();

    private async void InputFolderHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        if (await SelectFolderAsync() is StorageFolder storageFolder)
        {
            InputFolderTextBlock.Text = storageFolder.Path;
        }
    }

    private async void OutputFolderHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        if (await SelectFolderAsync() is StorageFolder storageFolder)
        {
            OutputFolderTextBlock.Text = storageFolder.Path;
        }
    }

    private async Task<StorageFolder?> SelectFolderAsync()
    {
        FolderPicker folderPicker = new();
        folderPicker.FileTypeFilter.Add("*");
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        return await folderPicker.PickSingleFolderAsync();
    }
}