using System;

namespace DNAGedLib.Models
{
    public partial class MatchKitName
    {       
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? PersonCount { get; set; }
    }
}