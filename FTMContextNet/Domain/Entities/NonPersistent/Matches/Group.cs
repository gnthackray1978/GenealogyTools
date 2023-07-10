using System.Collections.Generic;
using System.Linq;

namespace FTMContextNet.Domain.Entities.NonPersistent.Matches
{
    public class Group {
        public string IncludedTrees { get; set; }
        
        public int Id { get; set; }

        public int LatestTree { get; set; }

        public List<Item> Items { get; set; }

        public static Group Create(int id,  int personId, string origin, int yearFrom)
        {
            var mg = new Group
            {
                Id = id,
            };
            mg.Items.Add(new Item
            {
                Origin = origin,
                PersonId = personId,
                YearFrom = yearFrom

            });
             

            return mg;
        }
        
        public Group() {
            Items = new List<Item>(); 
        }

        public void AddPerson(int personId, string origin, int yearFrom)
        {
            if (!ContainsOrigin(origin))
            {
                Items.Add(new Item
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

            foreach (var p in Items)
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
        
        public bool Contains(int personId)
        {

            var personExists = false;

            Items.ForEach(f=>
            {
                if (f.PersonId == personId)
                    personExists = true;
            });

            return personExists;
        }

        public bool ContainsOrigin(string origin)
        {

            var originExists = false;

            Items.ForEach(f =>
            {
                if (f.Origin == origin)
                    originExists = true;
            });

            return originExists;
        }
        
    }
}
