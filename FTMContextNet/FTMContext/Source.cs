using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Source
    {
        public Source()
        {
            SourceLink = new HashSet<SourceLink>();
        }

        public int Id { get; set; }
        public int? MasterSourceId { get; set; }
        public string PageNumber { get; set; }
        public string Comment { get; set; }
        public int IncludeComment { get; set; }
        public string Footnote { get; set; }
        public string Abbreviated { get; set; }
        public int MediaLinkedCounts { get; set; }
        public string Pid { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual MasterSource MasterSource { get; set; }
        public virtual ICollection<SourceLink> SourceLink { get; set; }
    }
}
