using System;

namespace DNAGedLib
{

    public class MatchTreeEntry
    {
        public int Id { get; set; }

        public Guid MatchId { get; set; }

        public string Surname { get; set; }

        public string GivenName { get; set; }

        public string BirthString { get; set; }

        public int BirthInt { get; set; }

        public string DeathString { get; set; }

        public int DeathInt { get; set; }

        public string BirthPlace { get; set; }

        public string DeathPlace { get; set; }

        public int RelId { get; set; }

        public long PersonId { get; set; }

        public long FatherId { get; set; }

        public long MotherId { get; set; }

        public string Source { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
