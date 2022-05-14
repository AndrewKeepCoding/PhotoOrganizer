using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace PhotoOrganizings.ViewModels;

public partial class OutputFolderNodeViewModel : OutputNodeViewModelBase
{
    [ObservableProperty]
    private int _fileNodesCount = 0;

    public OutputFolderNodeViewModel(string name) : base(name)
    {
    }

    public ObservableCollection<OutputNodeViewModelBase> Children { get; private set; } = new();

    protected void AddFileNode(OutputFileNodeViewModel fileNode, string[] folderHierarchy)
    {
        string folderName = folderHierarchy.FirstOrDefault() ?? string.Empty;
        if (folderName.Length > 0)
        {
            OutputFolderNodeViewModel? folderNode = Children
                .OfType<OutputFolderNodeViewModel>()
                .Where(f => f.Name == folderName)
                .FirstOrDefault();

            if (folderNode is null)
            {
                folderNode = new OutputFolderNodeViewModel(Path.Combine(this.NodePath, folderName));
                Children.Add(folderNode);
            }

            folderNode.AddFileNode(fileNode, folderHierarchy[1..]);
        }
        else
        {
            Children.Add(fileNode);
        }

        FileNodesCount = GetFileNodesCount();
    }

    protected int GetFileNodesCount()
    {
        int fileNodesCount = 0;

        foreach (OutputNodeViewModelBase childNode in Children)
        {
            if (childNode is OutputFolderNodeViewModel folder)
            {
                fileNodesCount += folder.GetFileNodesCount();
            }
            else if (childNode is OutputFileNodeViewModel)
            {
                fileNodesCount++;
            }
        }

        return fileNodesCount;
    }
}