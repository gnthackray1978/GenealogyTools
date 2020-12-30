using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncRepository
    {
        public int Id { get; set; }
        public long FtmId { get; set; }
        public string AmtId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
