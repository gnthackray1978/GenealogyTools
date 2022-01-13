using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Relationship
    {
        public Relationship()
        {
            ChildRelationship = new HashSet<ChildRelationship>();
          //  Person = new HashSet<Person>();
        }

        public int Id { get; set; }
        public int? Person1Id { get; set; }
        public int? Person2Id { get; set; }
        public int RelType { get; set; }
        public int Status { get; set; }
        public int Nature { get; set; }
        public int Order1 { get; set; }
        public int Order2 { get; set; }
        public bool Private { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual Person Person1 { get; set; }
        public virtual Person Person2 { get; set; }
        public virtual ICollection<ChildRelationship> ChildRelationship { get; set; }
        //public virtual ICollection<Person> Person { get; set; }
    }
}
