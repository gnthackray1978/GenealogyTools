using System;
using System.Collections.Generic;

namespace DNAGedLib.Models
{
    public partial class Persons
    {
        public Persons()
        {
            MatchTrees = new HashSet<MatchTrees>();
        }

        public long Id { get; set; }
        public string ChristianName { get; set; }
        public string Surname { get; set; }
        public long? FatherId { get; set; }
        public long? MotherId { get; set; }
        public string BirthDate { get; set; }
        public int? BirthYear { get; set; }
        public string BirthPlace { get; set; }
        public string BirthCounty { get; set; }
        public string BirthCountry { get; set; }
        public string DeathDate { get; set; }
        public int? DeathYear { get; set; }
        public string DeathPlace { get; set; }
        public string DeathCounty { get; set; }
        public string DeathCountry { get; set; }
        public string Memory { get; set; }
        public bool RootsEntry { get; set; }
        public bool Fix { get; set; }
        public bool EnglishParentsChecked { get; set; }
        public bool AmericanParentsChecked { get; set; }
        public bool CountyUpdated { get; set; }
        public bool CountryUpdated { get; set; }
        public DateTime? CreatedDate { get; set; }
        public ICollection<MatchTrees> MatchTrees { get; set; }
    }
}
