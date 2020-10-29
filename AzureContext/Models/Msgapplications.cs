using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Msgapplications
    {
        public Msgapplications()
        {
            MsgapplicationMapGroup = new HashSet<MsgapplicationMapGroup>();
        }

        public int Id { get; set; }
        public string ApplicationName { get; set; }
        public bool Restricted { get; set; }

        public virtual ICollection<MsgapplicationMapGroup> MsgapplicationMapGroup { get; set; }
    }
}
