using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class MediaFile
    {
        public MediaFile()
        {
            MediaLink = new HashSet<MediaLink>();
            Person = new HashSet<Person>();
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileCaption { get; set; }
        public string FileDescription { get; set; }
        public string FileDate { get; set; }
        public byte[] Thumbnail { get; set; }
        public DateTime ModifiedTime { get; set; }
        public Guid Guid { get; set; }
        public string Pid { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsPrivate { get; set; }
        public string Uid { get; set; }

        public virtual ICollection<MediaLink> MediaLink { get; set; }
        public virtual ICollection<Person> Person { get; set; }
    }
}
