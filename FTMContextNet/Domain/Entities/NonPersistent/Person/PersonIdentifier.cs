
using System;
using System.Collections.Generic;
using GeoCoordinatePortable;
using GoogleMapsHelpers;

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
            bool? validBirthDates = null;
            bool? validLocations = null;

            if (other == null) 
                return false;

            if (this.Origin == other.Origin)
                return false;

            if (this.NameKey != other.NameKey)
                return false;

            if (this.SurnameKey != other.SurnameKey)
                return false;

            //if we have valid birth dates for comparison
            if (this.BirthYearFrom > 0 && this.BirthYearTo > 0 
                                       && other.BirthYearFrom > 0 && other.BirthYearTo > 0)
            {
                validBirthDates = MatchBirthYear(other);

                if (!validBirthDates.Value)
                    return false;
            }

            // we need either a valid county or valid coords for both
            if ((other.IsValidCoords() && this.IsValidCoords()) ||
                (other.IsValidCounty() && this.IsValidCounty()))
            {
                validLocations = MatchLocations(other);

                if (!validLocations.Value)
                    return false;
            }

          
            //if the name and the origin match then we'll say they're equal at this point.

            return true;
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
            if (p2.IsValidCoords() && this.IsValidCoords())
            {
                var d = Point.CalcDistance(p2.Lat, p2.Lng, this.Lat, this.Lng);

                return d <=5 ;
            }

            if (p2.IsValidCounty() && this.IsValidCounty())
            {
                return this.County == p2.County;
            }
           
            return true;
        }

        public bool IsValidCoords()
        {
            return (this.Lat != 0 && this.Lng != 0);
        }

        public bool IsValidCounty()
        {
            return !string.IsNullOrEmpty(this.County);
        }

    }
}
