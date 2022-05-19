using System.IO;

namespace PhotoOrganizings.ViewModels;

public class OutputFileNodeViewModel : OutputNodeViewModelBase
{
    public OutputFileNodeViewModel(string path) : base(path)
    {
        ParentPath = Path.GetDirectoryName(path);
    }

    public string? ParentPath { get; }
}