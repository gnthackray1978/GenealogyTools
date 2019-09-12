using System;
using System.Collections.Generic;
using System.Text;

namespace DNAGedLib.Models
{
    public partial class PersonGroups
    {
        public PersonGroups()
        {

        }

        public int Id { get; set; }
        public string GroupingKey { get; set; }
        public long PersonId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PersonGroupCount { get; set; }
        public int PersonGroupId { get; set; }
        public int PersonGroupIndex { get; set; }
        public string Description { get; set; }
        

    }
}
