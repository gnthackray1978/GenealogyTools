using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SourceLink
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public int SourceId { get; set; }
        public bool IsSource { get; set; }
        public int Stars { get; set; }
        public int CalcFlags { get; set; }
        public bool Calculated { get; set; }
        public string Justification { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual Source Source { get; set; }
    }
}
