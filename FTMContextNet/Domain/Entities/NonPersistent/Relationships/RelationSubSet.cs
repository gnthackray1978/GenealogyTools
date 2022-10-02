
using FTM.Dates;

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

        public bool MatchEither(int groupId)
        {
            if (Person1Id.GetValueOrDefault() == groupId || Person2Id.GetValueOrDefault() == groupId)
            {
                return true;
            }

            return false;
        }

        public int GetOtherSide(int groupId)
        {
            var potentialId = Person2Id.GetValueOrDefault();

            if (Person2Id == groupId)
            {
                potentialId = Person1Id.GetValueOrDefault();
            }

            return potentialId;
        }

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
