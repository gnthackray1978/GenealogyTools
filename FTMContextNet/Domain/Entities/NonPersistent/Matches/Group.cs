using System.Collections.Generic;
using System.Linq;

namespace FTMContextNet.Domain.Entities.NonPersistent.Matches
{
    public class Group {
        public string IncludedTrees { get; set; }
        
        public int Id { get; set; }

        public int LatestTree { get; set; }

        public List<Item> Items { get; set; }

        public static Group Create(int id, Item item)
        {
            var mg = new Group
            {
                Id = id,
            };
            mg.Items.Add(item);
            
            return mg;
        }
        
        public Group() {
            Items = new List<Item>(); 
        }

        public void AddRange(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Add(Item item)
        {
            if (!ContainsOrigin(item.Origin))
            {
               Items.Add(item);
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
