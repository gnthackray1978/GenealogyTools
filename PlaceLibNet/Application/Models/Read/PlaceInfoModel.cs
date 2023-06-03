using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceLibNet.Application.Models.Read
{
    public class PlaceInfoModel
    {
        public int Unsearched { get; set; }

        public int NotFound { get; set; }

        public int BadLocationsCount { get; set; }

        public int PlacesCount { get; set; }
         
    }
}
