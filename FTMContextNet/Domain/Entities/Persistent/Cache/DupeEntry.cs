namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class DupeEntry
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Ident { get; set; }
        public string Origin { get; set; }
        public int BirthYearFrom { get; set; }
        public int BirthYearTo { get; set; }
        public string Location { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public int ImportId { get; set; }

        public int UserId { get; set; }
    }
}