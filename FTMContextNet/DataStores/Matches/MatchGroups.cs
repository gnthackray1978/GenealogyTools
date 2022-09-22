
using System.Collections.Generic;
using System.Linq;

namespace FTMContext
{
    public class MatchGroups
    {
        public List<MatchGroup> Groups { get; set; } = new List<MatchGroup>();

        public MatchGroup FindByPersonId(int personId)
        {
            foreach (var matchGroup in Groups)
            {
                if (matchGroup.Contains(personId))
                {
                    return matchGroup;
                }
            }

            return null;
        }

        public MatchGroup CreateGroup(int personId, string origin, int yearFrom)
        {
            var newGroup = MatchGroup.Create(Groups.Count + 1, personId, origin, yearFrom);

            return newGroup;
        }

        public void SetAggregates()
        {
            foreach (var mg in Groups)
            {
                mg.setAggregates();
            }
        }

        public void SaveGroup(MatchGroup group)
        {
            bool groupExists= false;

            Groups.ForEach(f=>
            {
                if (f.ID == @group.ID)
                {
                    groupExists = true;
                }
            });


            //if it's a new group and we found dupes
            if (group.Persons.Count() > 1 && !groupExists)
            {
                Groups.Add(group);
            }
            
        }
    }
}