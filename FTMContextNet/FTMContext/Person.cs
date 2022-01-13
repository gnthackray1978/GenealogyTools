using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Person
    {
        public Person()
        {
            ChildRelationship = new HashSet<ChildRelationship>();
            PersonExternal = new HashSet<PersonExternal>();
            RelationshipPerson1 = new HashSet<Relationship>();
            RelationshipPerson2 = new HashSet<Relationship>();
        }

        public int Id { get; set; }
        public Guid PersonGuid { get; set; }
        public bool Private { get; set; }
        public int? NameFactId { get; set; }
        public string Sex { get; set; }
        public int? SexFactId { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public int? BirthPlaceId { get; set; }
        public int? BirthFactId { get; set; }
        public string DeathDate { get; set; }
        public string DeathPlace { get; set; }
        public int? DeathPlaceId { get; set; }
        public int? DeathFactId { get; set; }
        public string MarriageDate { get; set; }
        public string MarriagePlace { get; set; }
        public int? MarriagePlaceId { get; set; }
        public int? MarriageFactId { get; set; }
        public int? PreferredRelId { get; set; }
        public int? PreferredMediaFileId { get; set; }
        public int MediaLinkedCounts { get; set; }
        public int LinkedCounts { get; set; }
        public int Sequence { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string NameSuffix { get; set; }
        public string FullName { get; set; }
        public string FullNameReversed { get; set; }
        public int? BirthDateSort1 { get; set; }
        public int? BirthDateSort2 { get; set; }
        public int? BirthDateModifier1 { get; set; }
        public int? BirthDateModifier2 { get; set; }
        public int? BirthDateYear { get; set; }
        public int? DeathDateSort1 { get; set; }
        public int? DeathDateSort2 { get; set; }
        public int? DeathDateModifier1 { get; set; }
        public int? DeathDateModifier2 { get; set; }
        public int? DeathDateYear { get; set; }
        public int? MarriageDateSort1 { get; set; }
        public int? MarriageDateSort2 { get; set; }
        public int? MarriageDateModifier1 { get; set; }
        public int? MarriageDateModifier2 { get; set; }
        public int? MarriageDateYear { get; set; }
        public string Uid { get; set; }

        public virtual Fact BirthFact { get; set; }
        public virtual Place BirthPlaceNavigation { get; set; }
        public virtual Fact DeathFact { get; set; }
        public virtual Place DeathPlaceNavigation { get; set; }
        public virtual Fact MarriageFact { get; set; }
        public virtual Place MarriagePlaceNavigation { get; set; }
        public virtual Fact NameFact { get; set; }
        public virtual MediaFile PreferredMediaFile { get; set; }
        //public virtual Relationship PreferredRel { get; set; }
       
        public virtual Fact SexFact { get; set; }

        public virtual ICollection<ChildRelationship> ChildRelationship { get; set; }
        public virtual ICollection<PersonExternal> PersonExternal { get; set; }
        public virtual ICollection<Relationship> RelationshipPerson1 { get; set; }
        public virtual ICollection<Relationship> RelationshipPerson2 { get; set; }
    }
}
