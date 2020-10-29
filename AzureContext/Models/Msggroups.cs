using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Msggroups
    {
        public Msggroups()
        {
            MsgapplicationMapGroup = new HashSet<MsgapplicationMapGroup>();
            MsgfunctionMapGroup = new HashSet<MsgfunctionMapGroup>();
            MsggroupMapUser = new HashSet<MsggroupMapUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MsgapplicationMapGroup> MsgapplicationMapGroup { get; set; }
        public virtual ICollection<MsgfunctionMapGroup> MsgfunctionMapGroup { get; set; }
        public virtual ICollection<MsggroupMapUser> MsggroupMapUser { get; set; }
    }
}
