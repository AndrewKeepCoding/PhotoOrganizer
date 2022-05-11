using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using PhotoOrganizings.ViewModels;

namespace PhotoOrganizings.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        this.InitializeComponent();
    }

    private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }
}