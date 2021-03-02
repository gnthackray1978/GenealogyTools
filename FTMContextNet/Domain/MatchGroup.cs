using System.Collections.Generic;

namespace FTMContext
{
    public class MatchGroup {

        public MatchGroup() {
            Persons = new List<int>();
            Origins = new List<string>();
        }

        public int ID { get; set; }

        public List<int> Persons { get; set; }
        public List<string> Origins { get; set; }

        public bool Contains(int personId) {
            return Persons.Contains(personId);
        }
        public string IncludedTrees { get; set; }
        public int LatestTree { get; set; }
    }
}
