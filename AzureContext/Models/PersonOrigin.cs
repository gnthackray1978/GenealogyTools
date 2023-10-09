namespace AzureContext.Models;

public partial class PersonOrigin
{
    public int Id { get; set; }
    public int PersonId { get; set; } 
    public string Origin { get; set; }
    public int? ImportId { get; set; }
    public bool DirectAncestor { get; set; }
    public int? UserId { get; set; }
        
}

public partial class IgnoreList
{
    public int Id { get; set; }
    public string Person1 { get; set; }
    public string Person2 { get; set; }
}