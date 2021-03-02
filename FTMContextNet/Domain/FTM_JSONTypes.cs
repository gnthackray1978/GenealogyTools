using System;
using System.Collections.Generic;
using System.Text;

namespace FTMContext
{
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Bounds
    {
        public double south { get; set; }
        public double west { get; set; }
        public double north { get; set; }
        public double east { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public double south { get; set; }
        public double west { get; set; }
        public double north { get; set; }
        public double east { get; set; }
    }

    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class PlusCode
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
        public PlusCode plus_code { get; set; }
    }

    public class Root
    {
        public int id { get; set; }
        public List<Result> results { get; set; }
    }


}
