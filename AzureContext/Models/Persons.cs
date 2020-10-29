using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Persons
    {
        public Guid PersonId { get; set; }
        public int? MotherId { get; set; }
        public int? FatherId { get; set; }
        public bool? IsMale { get; set; }
        public string ChristianName { get; set; }
        public string Surname { get; set; }
        public string BirthLocation { get; set; }
        public string BirthDateStr { get; set; }
        public string BaptismDateStr { get; set; }
        public string DeathDateStr { get; set; }
        public string DeathLocation { get; set; }
        public string FatherChristianName { get; set; }
        public string FatherSurname { get; set; }
        public string MotherChristianName { get; set; }
        public string MotherSurname { get; set; }
        public string Notes { get; set; }
        public string Source { get; set; }
        public int? BirthInt { get; set; }
        public int? BapInt { get; set; }
        public int? DeathInt { get; set; }
        public string DeathCounty { get; set; }
        public string BirthCounty { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateLastEdit { get; set; }
        public string OrigSurname { get; set; }
        public string OrigFatherSurname { get; set; }
        public string OrigMotherSurname { get; set; }
        public string Occupation { get; set; }
        public string ReferenceLocation { get; set; }
        public string ReferenceDateStr { get; set; }
        public int? ReferenceDateInt { get; set; }
        public string SpouseName { get; set; }
        public string SpouseSurname { get; set; }
        public string FatherOccupation { get; set; }
        public Guid? BirthLocationId { get; set; }
        public Guid? DeathLocationId { get; set; }
        public Guid? ReferenceLocationId { get; set; }
        public string UniqueRef { get; set; }
        public int? TotalEvents { get; set; }
        public int? EventPriority { get; set; }
        public int? EstBirthYearInt { get; set; }
        public int? EstDeathYearInt { get; set; }
        public bool? IsEstBirth { get; set; }
        public bool? IsEstDeath { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
