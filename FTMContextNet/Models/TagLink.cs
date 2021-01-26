using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class TagLink
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
