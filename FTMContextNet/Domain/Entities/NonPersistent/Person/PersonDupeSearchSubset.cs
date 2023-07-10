
using System;

namespace FTMContext
{
    public class PersonDupeSearchSubset : IEquatable<PersonDupeSearchSubset>
    {
        public int Id { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public PersonDataObj Fact { get; set; }


        public bool Equals(PersonDupeSearchSubset other)
        {
            var yearMatch = this.Fact.MatchBirthYear(other.Fact);

            var locationMatch = this.Fact.MatchLocations(other.Fact);

            var originMatch = this.Fact.Origin == other.Fact.Origin;

            var name = this.GivenName == other.GivenName;

            var surname = this.FamilyName == other.FamilyName;

            return name && surname && yearMatch && locationMatch && !originMatch;
        }
    }
}
