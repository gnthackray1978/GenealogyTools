using System;

namespace DNAGedLib.Models
{
    public partial class MatchDetail
    {
        public int Id { get; set; }
        public Guid? TestGuid { get; set; }
        public Guid MatchGuid { get; set; }
        public int? SharedSegment { get; set; }

        public MatchGroups MatchGu { get; set; }
    }
}
