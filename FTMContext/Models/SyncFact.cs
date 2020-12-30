using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncFact
    {
        public int Id { get; set; }
        public string FtmId { get; set; }
        public string AmtId { get; set; }
        public long OwnerId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual SyncPerson Owner { get; set; }
    }
}
