using System.Collections.Generic;

namespace FTMContext
{
    public class PersonDataObj {

        public PersonDataObj() {
            Counties = new List<string>();
        }


        public List<string> Counties { get; set; }
        public int BirthYearFrom { get; set; }
        public int BirthYearTo { get; set; }
        public string Origin { get; set; }

        public bool MatchBirthYear(PersonDataObj p2) {


            if (p2.BirthYearFrom > this.BirthYearTo) return false;

            if (p2.BirthYearTo < this.BirthYearFrom) return false;

            return true;
        }

        public bool MatchLocations(PersonDataObj p2)
        {
            foreach (var p2C in p2.Counties) {
                foreach (var p1C in this.Counties) {
                    if (p2C == p1C)
                        return true;
                }
            }
 
            return false;
        }

    }
}
