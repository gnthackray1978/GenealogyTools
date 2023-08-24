
using System;
using System.Collections.Generic;

namespace FTMContextNet.Domain.Entities.NonPersistent.Person
{
    public class PersonIdentifier : IEquatable<PersonIdentifier>
    {
        public int Id { get; set; }
        public string NameKey { get; set; }
        public string SurnameKey { get; set; }
     //   public List<string> AssociatedLocations { get; set; }

        public string County { get; set; }

        public double Lng { get; set; }

        public double Lat { get; set; }

        public int BirthYearFrom { get; set; }
        public int BirthYearTo { get; set; }
        public string Origin { get; set; }
        public string Surname { get; set; }


        public bool Equals(PersonIdentifier other)
        {
            if (other == null) 
                return false;

            if (this.Origin == other.Origin)
                return false;

            if (this.NameKey != other.NameKey)
                return false;

            if (this.SurnameKey != other.SurnameKey)
                return false;

            if (!MatchBirthYear(other))
                return false;
 
            if (!MatchLocations(other))
                return false;

            return false;
        }

        public static PersonIdentifier Create(int id, int birthFrom, int birthTo, string origin, 
            string county, double lng, double lat, string surname, string firstName)
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
                County = county,
                Lng = lng,
                Lat = lat

            };


            //var trimmedLocations = linkedLocations.Trim();

            //if (trimmedLocations.Contains(","))
            //    returnObj.AssociatedLocations.AddRange(trimmedLocations.Split(','));
            //else
            //    returnObj.AssociatedLocations.Add(trimmedLocations);

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
            //foreach (var p2C in p2.AssociatedLocations) {
            //    foreach (var p1C in this.AssociatedLocations) {
            //        if (p2C == p1C)
            //            return true;
            //    }
            //}
 
            return false;
        }
    }
}
