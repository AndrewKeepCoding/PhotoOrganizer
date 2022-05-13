using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizings.ViewModels;
using System;

namespace PhotoOrganizings.Views;

public class OutputNodeTemplateSelector : DataTemplateSelector
{
    public DataTemplate? RootFolderNodeTemplate { get; set; }
    public DataTemplate? FolderNodeTemplate { get; set; }
    public DataTemplate? FileNodeTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return item switch
        {
            OutputRootFolderNodeViewModel => RootFolderNodeTemplate,
            OutputFolderNodeViewModel => FolderNodeTemplate,
            OutputFileNodeViewModel => FileNodeTemplate,
            _ => throw new ArgumentException(item.ToString()),
        };
    }
}