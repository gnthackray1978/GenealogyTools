using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Parishs
    {
        public Guid ParishId { get; set; }
        public string ParishName { get; set; }
        public string ParishRegistersDeposited { get; set; }
        public string ParishNotes { get; set; }
        public string ParentParish { get; set; }
        public int? ParishStartYear { get; set; }
        public int? ParishEndYear { get; set; }
        public string ParishCounty { get; set; }
        public decimal? ParishX { get; set; }
        public decimal? ParishY { get; set; }
    }
}
