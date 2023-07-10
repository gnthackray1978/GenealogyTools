using System.Collections.Generic;
using System.Linq;
using FTMContext;

namespace FTMContextNet.Domain.Entities.NonPersistent.Matches
{
    public class GroupCollection
    {
        public List<Group> Groups { get; set; } = new List<Group>();

        public Group FindByPersonId(int personId)
        {
            return Groups.FirstOrDefault(matchGroup => matchGroup.Contains(personId));
        }

        public Group CreateGroup(int personId, string origin, int yearFrom)
        {
            var newGroup = Group.Create(Groups.Count + 1, personId, origin, yearFrom);

            return newGroup;
        }

        public void SetAggregates()
        {
            foreach (var mg in Groups)
            {
                mg.setAggregates();
            }
        }

        public void SaveGroup(Group group)
        {
            bool groupExists= false;

            Groups.ForEach(f=>
            {
                if (f.Id == @group.Id)
                {
                    groupExists = true;
                }
            });


            //if it's a new group and we found dupes
            if (group.Items.Count() > 1 && !groupExists)
            {
                Groups.Add(group);
            }
            
        }
    }
}