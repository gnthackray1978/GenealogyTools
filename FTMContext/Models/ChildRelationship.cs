using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class ChildRelationship
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int RelationshipId { get; set; }
        public int Nature1 { get; set; }
        public int Nature2 { get; set; }
        public int ChildOrder { get; set; }
        public bool Private { get; set; }
        public bool Preferred { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual Person Person { get; set; }
        public virtual Relationship Relationship { get; set; }
    }
}
