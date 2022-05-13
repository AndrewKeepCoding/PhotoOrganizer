using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Linq;

namespace PhotoOrganizings.ViewModels;

[ObservableObject]
public abstract partial class OutputNodeViewModelBase
{
    public OutputNodeViewModelBase(string path)
    {
        NodePath = path;
        string[] nodes = NodePath.Split(Path.DirectorySeparatorChar);
        Name = nodes.Last();
        ParentsHierarchy = nodes.SkipLast(1).ToArray();
    }

    public string Name { get; }
    public string NodePath { get; }
    public string[] ParentsHierarchy { get; }
}