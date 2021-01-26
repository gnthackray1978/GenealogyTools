namespace DNAGedLib.Models
{
    public interface IPersons
    {
        long Id { get; set; }
        string BirthPlace { get; set; }
        string BirthCounty { get; set; }
        string BirthCountry { get; set; }
        bool CountyUpdated { get; set; }
        bool CountryUpdated { get; set; }
        long? FatherId { get; set; }
        long? MotherId { get; set; }
        bool EnglishParentsChecked { get; set; }
        bool AmericanParentsChecked { get; set; }
    }
}