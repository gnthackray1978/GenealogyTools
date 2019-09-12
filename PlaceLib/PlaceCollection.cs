using System.Collections.Generic;
using System.Linq;

namespace PlaceLib
{
    public class PlaceCollection : List<PlaceDto>
    {
        public new bool Contains(PlaceDto place)
        {
            var found = false;
            foreach (var mr in this)
            {
                if (mr.Place == place.Place && mr.County == place.County)
                {
                    found = true;
                }
            }

            return found;
        }

        public new string ToString()
        {
            string record = "";

            foreach (var mr in this)
            {
                record += mr.Place + " ,";
            }

            if (record.Last() == ',')
            {
                record = record.TrimEnd(',');
            }

            return  Count + " "+ record;
        }
    }
}