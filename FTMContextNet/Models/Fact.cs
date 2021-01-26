using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Fact
    {
        public Fact()
        {
            PersonBirthFact = new HashSet<Person>();
            PersonDeathFact = new HashSet<Person>();
            PersonMarriageFact = new HashSet<Person>();
            PersonNameFact = new HashSet<Person>();
            PersonSexFact = new HashSet<Person>();
        }

        public int Id { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public int FactTypeId { get; set; }
        public bool Private { get; set; }
        public bool Preferred { get; set; }
        public string Date { get; set; }
        public int? PlaceId { get; set; }
        public string Text { get; set; }
        public int MediaLinkedCounts { get; set; }
        public int SourceLinkedCounts { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Status { get; set; }
        public int? DateSort1 { get; set; }
        public int? DateSort2 { get; set; }
        public int? DateModifier1 { get; set; }
        public int? DateModifier2 { get; set; }
        public string Uid { get; set; }

        public virtual FactType FactType { get; set; }
        public virtual Place Place { get; set; }
        public virtual ICollection<Person> PersonBirthFact { get; set; }
        public virtual ICollection<Person> PersonDeathFact { get; set; }
        public virtual ICollection<Person> PersonMarriageFact { get; set; }
        public virtual ICollection<Person> PersonNameFact { get; set; }
        public virtual ICollection<Person> PersonSexFact { get; set; }
    }
}
