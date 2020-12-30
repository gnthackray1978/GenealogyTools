using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class FactType
    {
        public FactType()
        {
            Fact = new HashSet<Fact>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Abbreviation { get; set; }
        public int FactCategory { get; set; }
        public int FactClass { get; set; }
        public string Tag { get; set; }
        public string SentenceFormat { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual ICollection<Fact> Fact { get; set; }
    }
}
