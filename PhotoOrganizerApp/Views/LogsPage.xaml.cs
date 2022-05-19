using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using PhotoOrganizings.ViewModels;
using PhotoOrganizings.Helpers;
using Serilog;
using Serilog.Sinks.WinUi3.LogViewModels;
using Microsoft.UI.Xaml.Media;
using Serilog.Templates;
using Serilog.Events;
using Microsoft.UI;
using Serilog.Sinks.WinUi3;

namespace PhotoOrganizings.Views;

public sealed partial class LogsPage : Page
{
    private SerilogItemsRepeaterLogBroker? _logBroker;

    public LogsPage()
    {
        ViewModel = App.GetService<LogsViewModel>();
        this.InitializeComponent();
        ConfigureLogger();
    }

    public LogsViewModel ViewModel { get; }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    private void ConfigureLogger()
    {
        App.Current.Resources.TryGetValue("DefaultTextForegroundThemeBrush", out object defaultTextForegroundThemeBrush);

        _logBroker = new SerilogItemsRepeaterLogBroker(
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
}