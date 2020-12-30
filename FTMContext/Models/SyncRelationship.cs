using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncRelationship
    {
        public int Id { get; set; }
        public string FtmId { get; set; }
        public string AmtId { get; set; }
        public long PersonId { get; set; }
        public long OtherId { get; set; }
        public long OwnerId { get; set; }
        public int Type { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual SyncPerson Person { get; set; }
    }
}
