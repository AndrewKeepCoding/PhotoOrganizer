using System.Collections.ObjectModel;

namespace PhotoOrganizings.ViewModels;

public class OutputFolderNodeViewModel : OutputNodeViewModelBase
{
    public OutputFolderNodeViewModel(string name) : base(name)
    {
    }

    public ObservableCollection<OutputNodeViewModelBase> Children { get; private set; } = new();
    //public ObservableCollection<OutputFolderNodeViewModel> FolderChildren { get; private set; } = new();
    //public ObservableCollection<OutputFileNodeViewModel> FileChildren { get; private set; } = new();

    public void AddFolderNode(OutputFolderNodeViewModel folderNode)
    {
        //FolderChildren ??= new();
        //FolderChildren.Add(folderNode);
        Children.Add(folderNode);
    }

    public void AddFileNode(OutputFileNodeViewModel fileNode)
    {
        //FileChildren ??= new();
        //FileChildren.Add(fileNode);
        Children.Add(fileNode);
    }
}