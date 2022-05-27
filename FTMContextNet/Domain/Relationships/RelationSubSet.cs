using MyFamily.Shared;

namespace FTMContext
{
    public class RelationSubSet {
        public int Id { get; set; }
        public int? Person1Id { get; set; }
        public int? Person2Id { get; set; }

        public int LinkId { get; set; }

        public Date Date { get; set; }
        public int? PlaceId { get; set; }
        public string Text { get; set; }

        public string Origin { get; set; }

        public string PlaceName { get; set; }

        public static bool ValidYear(Date date)
        {
            if (date == null) return false;

            if (date.Year != null && date.HasYear())
            {
                return true;
            }

            return false;
        }
    }
}
