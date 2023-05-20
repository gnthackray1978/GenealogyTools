using QuickGed.Domain;

namespace QuickGed.Types;

public class Node
{
    protected readonly INodeTypeCalculator _nodeTypeCalculator;
    public Node(int id, INodeTypeCalculator nodeTypeCalculator)
    {
        _nodeTypeCalculator = nodeTypeCalculator;
        Id = id;
    }

    public int Id { get; set; }
    public int FatherId { get; set; }
    public int MotherId { get; set; }

    public virtual string FullName { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public bool IsParent { get; set; }

    public bool IsDirectAncestor { get; set; }

    public bool IsRootPerson { get; set; }

    public bool IsLinkNode { get; set; }

    public List<Node> Spouses { get; set; } = new List<Node>();
    public List<Node> Siblings { get; set; } = new List<Node>();
    public List<Node> Children { get; set; } = new List<Node>();
}