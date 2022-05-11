using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using PhotoOrganizings.ViewModels;

namespace PhotoOrganizings.Views;

public sealed partial class LogsPage : Page
{
    public LogsPage()
    {
        ViewModel = App.GetService<LogsViewModel>();
        this.InitializeComponent();
    }

    public LogsViewModel ViewModel { get; }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }
}