using System.IO;
using System.Linq;

namespace PhotoOrganizings.ViewModels;

public class OutputRootFolderNodeViewModel : OutputFolderNodeViewModel
{
    public OutputRootFolderNodeViewModel(string path) : base(path)
    {
    }

    public void AddFileChild(OutputFileNodeViewModel fileNode)
    {
        if (fileNode.ParentsHierarchy is not null)
        {
            int rootDepth = ParentsHierarchy.Length + 1;
            OutputFolderNodeViewModel currentFolder = this;

            for (int i = rootDepth; i < fileNode.ParentsHierarchy.Length; i++)
            {
                string searchingFolderName = fileNode.ParentsHierarchy[i];
                OutputFolderNodeViewModel? searchingResult = currentFolder.Children?
                    .OfType<OutputFolderNodeViewModel>()
                    .Where(node => node.Name == searchingFolderName)
                    .FirstOrDefault();

                if (searchingResult is null)
                {
                    string newFolderNodePath = Path.Combine(fileNode.ParentsHierarchy[0..(i + 1)]);
                    searchingResult = new OutputFolderNodeViewModel(newFolderNodePath);
                    currentFolder.AddFolderNode(searchingResult);
                }

                currentFolder = searchingResult;
            }

            currentFolder.AddFileNode(fileNode);
        }
    }
}