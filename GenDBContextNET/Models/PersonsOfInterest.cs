using System;

namespace DNAGedLib.Models
{
    public partial class PersonsOfInterest
    {
        public PersonsOfInterest()
        {

        }

        public long Id { get; set; }
        public long PersonId { get; set; }
        public string ChristianName { get; set; }
        public string Surname { get; set; }
        public int? BirthYear { get; set; }
           
        public string BirthPlace { get; set; }
        public string BirthCounty { get; set; }
        public string BirthCountry { get; set; }
        public string TestDisplayName { get; set; }
        public string TestAdminDisplayName { get; set; }
        public string TreeURL { get; set; }
        public Guid TestGuid { get; set; }
        public double Confidence { get; set; }
        public double SharedCentimorgans { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool RootsEntry { get; set; }
        public string Memory { get; set; }
        public Guid KitId { get; set; }
        public string Name { get; set; }

    }
}