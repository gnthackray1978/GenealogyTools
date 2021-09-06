using System;
using System.Collections.Generic;

namespace FTMContext
{
    public class PersonDataObj {

        public static PersonDataObj Create(int birthFrom, int birthTo, string origin, string linkedLocations, string surname)
        {
            var returnObj = new PersonDataObj
            {
                BirthYearFrom = birthFrom,
                BirthYearTo = birthTo,
                Origin = origin,
                Surname =  surname
            };


            var trimmedLocations = linkedLocations.Trim();

            if (trimmedLocations.Contains(","))
                returnObj.AssociatedLocations.AddRange(trimmedLocations.Split(','));
            else
                returnObj.AssociatedLocations.Add(trimmedLocations);

            return returnObj;
        }

        public PersonDataObj() {
            AssociatedLocations = new List<string>();
        }


        public List<string> AssociatedLocations { get; set; }
        public int BirthYearFrom { get; set; }
        public int BirthYearTo { get; set; }
        public string Origin { get; set; }

        public string Surname { get; set; }

        public bool DoubleCheckSurname(PersonDataObj p2)
        {
            //hack because double metaphone is sometimes a bit rubbish.

            if (nameException(p2, "price"))
                return false;

            if (nameException(p2, "bond"))
                return false;

            if (nameException(p2, "todd"))
                return false;

            if (nameException(p2, "bretland"))
                return false;

            if (nameException(p2, "brace"))
                return false;

            if (nameException(p2, "holdway"))
                return false;

            if (nameException(p2, "horton"))
                return false;

            if (nameException(p2, "moor"))
                return false;

            if (nameException(p2, "underhill"))
                return false;

            if (nameException(p2, "brigham"))
                return false;

            if (nameException(p2, "wright"))
                return false;

            if (nameException(p2, "tempest"))
                return false;

            if (nameException(p2, "ade"))
                return false;

            if (nameException(p2, "barber"))
                return false;

            if (nameException(p2, "craze"))
                return false;

            if (nameException(p2, "green"))
                return false;

            if (nameException(p2, "payne"))
                return false;

            return true;
        }

        private bool nameException(PersonDataObj p2, string nameToCheck)
        {
            if (Surname.Trim().ToLower() == nameToCheck || p2.Surname.Trim().ToLower() == nameToCheck)
            {
                if (Surname.Trim().ToLower() == p2.Surname.Trim().ToLower())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public bool MatchBirthYear(PersonDataObj p2) {

            if (p2.BirthYearFrom == p2.BirthYearTo && BirthYearTo == BirthYearFrom)
            {
                return  Math.Abs(p2.BirthYearFrom - BirthYearFrom) <= 3;
            }

            if (p2.BirthYearFrom > this.BirthYearTo) return false;

            if (p2.BirthYearTo < this.BirthYearFrom) return false;

            return true;
        }

        public bool MatchLocations(PersonDataObj p2)
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
