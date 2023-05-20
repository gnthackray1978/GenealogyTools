using QuickGed.Domain;

namespace QuickGed.Types;

public class Person : Node, IPerson
{
    public Person(int id, INodeTypeCalculator nodeTypeCalculator) : base(id, nodeTypeCalculator)
    {
        
    }
    public void SetIsRootPerson()
    {
        IsRootPerson = _nodeTypeCalculator.IsRootPerson(Forename,FamilyName);
        IsLinkNode = _nodeTypeCalculator.IsLinkNode(Forename,FamilyName);
    }

    public string Sex { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int BirthYearFrom { get; set; }
    public int BirthYearTo { get; set; }
    public string BirthLocation { get; set; } = string.Empty;
    public string BirthNote { get; set; } = string.Empty;
    public int DeathYear { get; set; }
    public string DeathDate { get; set; } = string.Empty;
    public string DeathNote { get; set; } = string.Empty;

    public string DeathLocation { get; set; } = string.Empty;
    public string Forename { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public override string FullName => Forename + " " + FamilyName;
    public string Occupation { get; set; } = string.Empty;
    public int ResidenceYear { get; set; }
    public string ResidenceDate { get; set; } = string.Empty;
    public string Residence { get; set; } = string.Empty;
    public string ResidenceDescription { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public bool IsValidLocation()
    {
        return !string.IsNullOrEmpty(BirthLocation) || !string.IsNullOrEmpty(DeathLocation) || !string.IsNullOrEmpty(Residence);
    }



    public string AllLocations()
    {
        var result = "";

        if (!string.IsNullOrEmpty(BirthLocation))
            result += BirthLocation + ",";

        if (!string.IsNullOrEmpty(DeathLocation))
            result += DeathLocation + ",";

        if (!string.IsNullOrEmpty(Residence))
            result += Residence;

        result = result.TrimEnd(',');

        return result;
    }

}