using QuickGed.Types;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class FTMMarriage
    {
        public static FTMMarriage Create(RelationSubSet r)
        {
            return new FTMMarriage()
            {
                BrideId = r.Person1Id.GetValueOrDefault(),
                GroomId = r.Person2Id.GetValueOrDefault(),
                MarriageDateStr = r.DateStr,
                MarriageYear = r.DateYear,
                Notes = r.Text,
                Origin = r.Origin,
                MarriageLocation = r.PlaceName
            };
        }

        public int Id { get; set; }
        public int GroomId { get; set; }

        public int BrideId { get; set; }

        public string Origin { get; set; }

        public string MarriageLocation { get; set; }

        public string Notes { get; set; }

        public string MarriageDateStr { get; set; }

        public int MarriageYear { get; set; }

        public int ImportId { get; set; }

    }
}