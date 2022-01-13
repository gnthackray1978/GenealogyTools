using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncNote
    {
        public int Id { get; set; }
        public string AmtId { get; set; }
        public string FtmId { get; set; }
        public long OwnerId { get; set; }
        public int OwnerType { get; set; }
        public int IsResearchNote { get; set; }
        public int IsPrivate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
