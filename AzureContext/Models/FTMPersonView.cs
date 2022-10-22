namespace AzureContext.Models
{
    public partial class FTMPersonView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string Surname { get; set; }

        public int? BirthFrom { get; set; }
        public int? BirthTo { get; set; }

        public string BirthLocation { get; set; }
        public double? BirthLat { get; set; }
        public double? BirthLong { get; set; }

        public string AltLocationDesc { get; set; }
        public string AltLocation { get; set; }
        public double? AltLat { get; set; }
        public double? AltLong { get; set; }

        public int Origin { get; set; }
        public int? PersonId { get; set; }

        public int? FatherId { get; set; }

        public int? MotherId { get; set; }
        public bool? DirectAncestor { get; set; }
    }
}
