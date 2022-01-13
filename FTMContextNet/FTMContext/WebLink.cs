using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class WebLink
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }
    }
}
