﻿using System.Collections.Generic;
using System.Linq;
using FTMContextNet.Domain.Entities.NonPersistent.Matches;

namespace FTMContextNet.Domain.Collections.Grouping
{
    public class GroupCollection
    {
        public List<Group> Groups { get; set; } = new List<Group>();

        public Group FindById(int personId)
        {
            return Groups.FirstOrDefault(matchGroup => matchGroup.Contains(personId));
        }

        public Group CreateGroup(Item item)
        {
            var newGroup = Group.Create(Groups.Count + 1, item);

            return newGroup;
        }

        public void SetAggregates()
        {
            foreach (var mg in Groups)
            {
                mg.setAggregates();
            }
        }

        public void WriteGroup(Group group)
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