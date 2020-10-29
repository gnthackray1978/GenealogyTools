using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Msgfunctions
    {
        public Msgfunctions()
        {
            MsgfunctionMapGroup = new HashSet<MsgfunctionMapGroup>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ApplicationId { get; set; }

        public virtual ICollection<MsgfunctionMapGroup> MsgfunctionMapGroup { get; set; }
    }
}
