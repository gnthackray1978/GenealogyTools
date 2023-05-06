namespace QuickGed
{
    public interface IPerson
    {
        int Id { get; set; }
        string FamilyName { get; set; }
        string FullName { get; }
    }
}
