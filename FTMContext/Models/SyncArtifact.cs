using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncArtifact
    {
        public int Id { get; set; }
        public string FtmId { get; set; }
        public string AmtId { get; set; }
        public long Size { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
