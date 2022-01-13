using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncCitationReference
    {
        public int Id { get; set; }
        public string FtmId { get; set; }
        public string AmtId { get; set; }
        public long PersonId { get; set; }
        public long FactId { get; set; }
        public long CitationId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
