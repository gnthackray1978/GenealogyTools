using System.Collections.Generic;
using System.Linq;

namespace FTMContext
{
    public class MatchPerson
    {
        public int PersonId { get; set; }
        public string Origin { get; set; }
        public int YearFrom { get; set; }

    }

    public class MatchGroup {
        public string IncludedTrees { get; set; }
        
        public int ID { get; set; }

        public int LatestTree { get; set; }
        

        public static MatchGroup Create(int id,  int personId, string origin, int yearFrom)
        {
            var mg = new MatchGroup
            {
                ID = id,
            };
            mg.Persons.Add(new MatchPerson
            {
                Origin = origin,
                PersonId = personId,
                YearFrom = yearFrom

            });
             

            return mg;
        }



        public MatchGroup() {
            Persons = new List<MatchPerson>(); 
        }

        public void AddPerson(int personId, string origin, int yearFrom)
        {
            if (!ContainsOrigin(origin))
            {
                Persons.Add(new MatchPerson
                {
                    Origin = origin,
                    PersonId = personId,
                    YearFrom = yearFrom
                });
            }
        }

        public void setAggregates()
        {
            IncludedTrees = "";
            LatestTree = 0;

            List<string> origins = new List<string>();

            foreach (var p in Persons)
            {
                if (p.YearFrom > LatestTree)
                    LatestTree = p.YearFrom;
                 
                origins.Add(p.Origin);
            }

            foreach (var o in origins.OrderBy(o => o))
            {
                IncludedTrees += o;
            }
             
        }



        public List<MatchPerson> Persons { get; set; } 

        public bool Contains(int personId)
        {

            var personExists = false;

            Persons.ForEach(f=>
            {
                if (f.PersonId == personId)
                    personExists = true;
            });

            return personExists;
        }

        public bool ContainsOrigin(string origin)
        {

            var originExists = false;

            Persons.ForEach(f =>
            {
                if (f.Origin == origin)
                    originExists = true;
            });

            return originExists;
        }



    }
}
