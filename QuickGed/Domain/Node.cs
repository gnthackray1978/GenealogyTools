namespace QuickGed.Types;

public class Node
{
    public int Id { get; set; }
    public int FatherId { get; set; }
    public int MotherId { get; set; }

    public virtual string FullName { get; set; }
    public string Origin { get; set; }
    public bool IsParent { get; set; }

    public bool IsDirectAncestor { get; set; }

    public List<Node> Spouses { get; set; } = new List<Node>();
    public List<Node> Siblings { get; set; } = new List<Node>();
    public List<Node> Children { get; set; } = new List<Node>();
}