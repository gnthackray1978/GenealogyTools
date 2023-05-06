using QuickGed.Types;

namespace QuickGed;

public class GedDb
{
      
    public List<RelationSubSet> Relationships;
    public List<ChildRelationship> ChildRelationships;
    public List<PersonSubset> Persons;
    public Dictionary<int, PersonSubset> PersonDictionary;

    public Dictionary<int, List<Node>> ParentDictionary;

    public string FileName { get; set; }

    public long FileSize { get; set; }

    public GedDb()
    {
        ParentDictionary = new Dictionary<int, List<Node>>();
        Relationships = new List<RelationSubSet>();
        ChildRelationships = new List<ChildRelationship>();
        PersonDictionary = new Dictionary<int, PersonSubset>();
        Persons = new List<PersonSubset>();
    }

    public static GedDb Create()
    {
        var g = new GedDb();
            
        return g;
    }
}