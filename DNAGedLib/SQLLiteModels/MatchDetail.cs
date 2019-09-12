using System;

namespace DNAGedLib
{
    public class MatchDetail
    {
        public int Id { get; set; }

        public Guid TestGuid { get; set; }

        public Guid MatchGuid { get; set; }

        public int SharedSegment { get; set; }


    }
}
