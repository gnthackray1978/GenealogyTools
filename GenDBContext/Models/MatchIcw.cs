using System;

namespace DNAGedLib.Models
{
    public partial class MatchIcw
    {
        public int Id { get; set; }
        public Guid? MatchId { get; set; }
        public string MatchName { get; set; }
        public string MatchAdmin { get; set; }
        public Guid? Icwid { get; set; }
        public string Icwname { get; set; }
        public string Icqadmin { get; set; }
        public string Source { get; set; }

      //  public MatchGroups Match { get; set; }
    }
}
