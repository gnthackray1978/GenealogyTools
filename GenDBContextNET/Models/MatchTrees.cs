using System;

namespace DNAGedLib.Models
{
    public partial class MatchTrees
    {
        public int Id { get; set; }
        public Guid MatchId { get; set; }
        public long? RelId { get; set; }
        public long PersonId { get; set; }
        public DateTime? CreatedDate { get; set; }         
        public Persons Person { get; set; }
        public TreePersons TreePerson { get; set; }
    }
}
