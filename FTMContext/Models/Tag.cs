using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Tag
    {
        public Tag()
        {
            TagLink = new HashSet<TagLink>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
        public int? Color { get; set; }
        public bool Emphasize { get; set; }
        public int? TableId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual ICollection<TagLink> TagLink { get; set; }
    }
}
