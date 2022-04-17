using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Serilog.Events;
using Serilog.Sinks.WinUi3;
using Serilog.Sinks.WinUi3.LogViewModels;
using System;
using System.Collections.ObjectModel;

namespace PhotoOrganizer.Utilities;

public class ItemsRepeaterLogBroker : IWinUi3LogBroker
{
    private readonly ILogViewModelBuilder _logViewModelBuilder;

    private ObservableCollection<ILogViewModel> _logs = new();

    public ItemsRepeaterLogBroker(
            ItemsRepeater itemsRepeater,
            ScrollViewer scrollViewer,
            ILogViewModelBuilder logViewModelBuilder)
    {
        _logViewModelBuilder = logViewModelBuilder;

        LogCollectionView = new(_logs, true);
        itemsRepeater.SetBinding(ItemsRepeater.ItemsSourceProperty, new Binding() { Source = LogCollectionView });
        
        DispatcherQueue = itemsRepeater.DispatcherQueue;
        AddLogEvent = logEvent => LogCollectionView.Add(_logViewModelBuilder.Build(logEvent));

        LogCollectionView.VectorChanged += ((sender, e) =>
        {
            if (IsAutoScrollOn is true && sender is AdvancedCollectionView collectionView)
            {
                scrollViewer.ChangeView(
                    horizontalOffset: 0,
                    verticalOffset: scrollViewer.ScrollableHeight,
                    zoomFactor: 1,
                    disableAnimation: true);
            }
        });
    }

    public Action<LogEvent> AddLogEvent { get; }
    public DispatcherQueue DispatcherQueue { get; }
    public bool IsAutoScrollOn { get; set; }
    public AdvancedCollectionView LogCollectionView { get; set; }
}
