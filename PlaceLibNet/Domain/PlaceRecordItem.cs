using System;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;


namespace PlaceLibNet.Model
{
    public class PlaceRecordItem : IComparable<PlaceRecordItem>
    {
        public string Place { get; set; }

        public string PlaceRaw { get; set; }

        public int GoogleCacheId { get; set; }

        public int PlaceLibId { get; set; }

        public string Country { get; set; }

        public string County { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }

        public int CompareTo(PlaceRecordItem obj)
        {
            return this.Place.CompareTo(obj.Place);
        }

    }
}
