namespace PhotoOrganizings.ViewModels;

public class OutputRootFolderNodeViewModel : OutputFolderNodeViewModel
{
    public OutputRootFolderNodeViewModel(string path) : base(path)
    {
    }

    public void AddFileChild(OutputFileNodeViewModel fileNode)
    {
        int currentDepth = ParentsHierarchy.Length + 1;
        AddFileNode(fileNode, fileNode.ParentsHierarchy[currentDepth..]);
    }
}