
using System;
using System.Collections.Generic;

namespace FTMContextNet.Domain.Entities.NonPersistent.Person
{
    public class PersonIdentifier : IEquatable<PersonIdentifier>
    {
        public int Id { get; set; }
        public string NameKey { get; set; }
        public string SurnameKey { get; set; }
        public List<string> AssociatedLocations { get; set; }
        public int BirthYearFrom { get; set; }
        public int BirthYearTo { get; set; }
        public string Origin { get; set; }
        public string Surname { get; set; }


        public bool Equals(PersonIdentifier other)
        {
            var yearMatch = this.MatchBirthYear(this);

            var locationMatch = this.MatchLocations(this);

            var originMatch = this.Origin == other.Origin;

            var name = this.NameKey == other.NameKey;

            var surname = this.SurnameKey == other.SurnameKey;

            return name && surname && yearMatch && locationMatch && !originMatch;
        }

        public static PersonIdentifier Create(int id, int birthFrom, int birthTo, string origin, string linkedLocations, string surname, string firstName)
        {
            var returnObj = new PersonIdentifier
            {
                BirthYearFrom = birthFrom,
                BirthYearTo = birthTo,
                Origin = origin,
                Surname =  surname,
                Id = id,
                SurnameKey = Misc.Misc.MakeKey(firstName),
                NameKey = Misc.Misc.MakeKey(surname),
                AssociatedLocations = new List<string>()
            };


            var trimmedLocations = linkedLocations.Trim();

            if (trimmedLocations.Contains(","))
                returnObj.AssociatedLocations.AddRange(trimmedLocations.Split(','));
            else
                returnObj.AssociatedLocations.Add(trimmedLocations);

            return returnObj;
        }

        public bool MatchBirthYear(PersonIdentifier p2) {

            if (p2.BirthYearFrom == p2.BirthYearTo && BirthYearTo == BirthYearFrom)
            {
                return  Math.Abs(p2.BirthYearFrom - BirthYearFrom) <= 3;
            }

            if (p2.BirthYearFrom > this.BirthYearTo) return false;

            if (p2.BirthYearTo < this.BirthYearFrom) return false;

            return true;
        }

        public bool MatchLocations(PersonIdentifier p2)
        {
            foreach (var p2C in p2.AssociatedLocations) {
                foreach (var p1C in this.AssociatedLocations) {
                    if (p2C == p1C)
                        return true;
                }
            }
 
            return false;
        }
    }
}
