using System;
using System.Collections.Generic;

namespace DNAGedLib.Models
{
    public partial class MatchGroups
    {
        public MatchGroups()
        {
            //MatchDetail = new HashSet<MatchDetail>();
            //MatchIcw = new HashSet<MatchIcw>();
          //  MatchTrees = new HashSet<MatchTrees>();
        }

        public int Id { get; set; }
        public Guid? TestGuid { get; set; }
        public Guid MatchGuid { get; set; }
        public string TestDisplayName { get; set; }
        public string TestAdminDisplayName { get; set; }
        public int? TreeNodeCount { get; set; }
        public string GroupName { get; set; }
        public double? Confidence { get; set; }
        public double? SharedCentimorgans { get; set; }
        public int? SharedSegment { get; set; }
        public bool? Starred { get; set; }
        public bool? Viewed { get; set; }
        public bool? TreesPrivate { get; set; }
        public bool? HasHint { get; set; }
        public string Note { get; set; }
        public bool? UserPhoto { get; set; }
        public string TreeId { get; set; }

        //public ICollection<MatchDetail> MatchDetail { get; set; }
        //public ICollection<MatchIcw> MatchIcw { get; set; }
     //   public ICollection<MatchTrees> MatchTrees { get; set; }
    }
}
