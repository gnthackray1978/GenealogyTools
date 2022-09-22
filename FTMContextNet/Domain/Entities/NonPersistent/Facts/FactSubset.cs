
using FTM.Dates;

namespace FTMContext
{
    public class FactSubset
    {       
        public int Id { get; set; }
        public int LinkId { get; set; }
     //   public int LinkTableId { get; set; }
     //   public int FactTypeId { get; set; }
        public Date Date { get; set; }
        public int? PlaceId { get; set; }
        public string Text { get; set; }

        public static bool ValidYear(Date date) {
            if (date == null) return false;

            if (date.Year != null && date.HasYear())
            {
                return true;
            }

            return false;
        }
    }
}
