using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncArtifactReference
    {
        public int Id { get; set; }
        public string FtmId { get; set; }
        public string AmtId { get; set; }
        public int PersonId { get; set; }
        public int OwnerId { get; set; }
        public int Type { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
