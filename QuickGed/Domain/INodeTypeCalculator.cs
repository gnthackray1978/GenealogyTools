namespace QuickGed.Domain;

public interface INodeTypeCalculator
{
    bool IsRootPerson(string forename, string surname);
    bool IsRootPerson(string fullName);
    public bool IsLinkNode(string forename, string surname);
    public bool IsLinkNode(string fullName);
}