using Microsoft.UI.Xaml;

namespace PhotoOrganizings;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
    }

    public void SetBody(UIElement uiElement) => Body.Content = uiElement;

    public UIElement GetBody() => Body;
}