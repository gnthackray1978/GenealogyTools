using System;
using System.Collections.Generic;

namespace FTMContextNet.Domain.Entities.Source
{
    public interface IRelationship
    {
        int Id { get; set; }
        int? Person1Id { get; set; }
        int? Person2Id { get; set; }
    }

    public partial class Relationship : IRelationship
    {
        public Relationship()
        {
            ChildRelationship = new HashSet<ChildRelationship>();
        }

        public int Id { get; set; }
        public int? Person1Id { get; set; }
        public int? Person2Id { get; set; }
        public bool Private { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }
        
        public virtual ICollection<ChildRelationship> ChildRelationship { get; set; }
    }
}
