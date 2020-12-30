using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class MediaLink
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public int MediaFileId { get; set; }
        public bool Private { get; set; }
        public int FileOrder { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual MediaFile MediaFile { get; set; }
    }
}
