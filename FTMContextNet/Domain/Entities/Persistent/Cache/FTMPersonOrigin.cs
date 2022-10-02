namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class FTMPersonOrigin
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Origin { get; set; }

        public bool DirectAncestor { get; set; }

    }
}