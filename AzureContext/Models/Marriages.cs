using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Marriages
    {
        public Guid MarriageId { get; set; }
        public string MaleCname { get; set; }
        public string MaleSname { get; set; }
        public string MaleLocation { get; set; }
        public string MaleInfo { get; set; }
        public string FemaleCname { get; set; }
        public string FemaleSname { get; set; }
        public string FemaleLocation { get; set; }
        public string FemaleInfo { get; set; }
        public string Date { get; set; }
        public string MarriageLocation { get; set; }
        public int? YearIntVal { get; set; }
        public string MarriageCounty { get; set; }
        public string Source { get; set; }
        public string Witness1 { get; set; }
        public string Witness2 { get; set; }
        public string Witness3 { get; set; }
        public string Witness4 { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateLastEdit { get; set; }
        public string OrigMaleSurname { get; set; }
        public string OrigFemaleSurname { get; set; }
        public string MaleOccupation { get; set; }
        public string FemaleOccupation { get; set; }
        public bool? FemaleIsKnownWidow { get; set; }
        public bool? MaleIsKnownWidower { get; set; }
        public bool? IsBanns { get; set; }
        public bool? IsLicence { get; set; }
        public Guid? MarriageLocationId { get; set; }
        public Guid? MaleLocationId { get; set; }
        public Guid? FemaleLocationId { get; set; }
        public int? MaleBirthYear { get; set; }
        public int? FemaleBirthYear { get; set; }
        public string UniqueRef { get; set; }
        public int? TotalEvents { get; set; }
        public int? EventPriority { get; set; }
        public Guid? MaleId { get; set; }
        public Guid? FemaleId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
