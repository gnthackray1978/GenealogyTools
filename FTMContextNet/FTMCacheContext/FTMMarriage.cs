namespace FTMContext.Models
{
    public partial class FTMMarriage
    {
        public int Id { get; set; }
        public int GroomId { get; set; }

        public int BrideId { get; set; }

        public string Origin { get; set; }

        public string MarriageLocation { get; set; }

        public string Notes { get; set; }

        public string MarriageDateStr { get; set; }

        public int MarriageYear { get; set; }

        public int MarriageLocationId { get; set; }

    }
}