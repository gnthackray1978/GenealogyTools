using System;
using System.Collections.Generic;
using System.Linq;
using PlaceLib.Model;

namespace PlaceLib
{
    public class CountyDto
    {
        public string County { get; set; }
        public string Country { get; set; }
    }

    public class PlaceDto
    {
        public string Place { get; set; }
        public string County { get; set; }
        public string Country { get; set; }

    }

    public class PlaceOperations
    {

        public static List<CountyDto> GetCounties()
        {
            var counties = new List<CountyDto>();

            using (var placecontext = new PlacesContext())
            {
                counties = placecontext.Places.Select(s => new CountyDto()
                {
                    County = s.Ctyhistnm.ToLower(),
                    Country = s.Ctry15nm

                }).Distinct().ToList();
            }

            return counties;
        }

        public static List<PlaceDto> GetPlaces()
        {
            var places = new List<PlaceDto>();

            using (var placecontext = new PlacesContext())
            {
                places = placecontext.Places.Where(w => w.Ctyhistnm != "").Select(s => new PlaceDto()
                {
                    County = s.Ctyhistnm.ToLower(),
                    Country = s.Ctry15nm,
                    Place = s.Place15nm.ToLower()
                }).Distinct().ToList();
            }

            return places;
        }


        public static string FindCountry(string place)
        {

            var nation = "Unknown";



            if (Locationcheck(place, "Australia","Queensland","Tasmania")) nation = "Australia";
            if (CheckUSA(place)) nation = "USA";
            if (Locationcheck(place, "France")) nation = "France";
            if (Locationcheck(place, "Ukraine")) nation = "Ukraine";
            if (Locationcheck(place, "India")) nation = "India";
            if (Locationcheck(place, "Hungary")) nation = "Hungary";
            if (Locationcheck(place, "Russia")) nation = "Russia";
            if (Locationcheck(place, "Greece")) nation = "Greece";
            if (Locationcheck(place, "Germany",
                "Prussia", 
                "Deutschland", 
                "Württemberg", 
                "Hesse",
                "Hannover",
                "Damstadt",
                "Kassel",
                "Wurtemburg",
                "Saarland",
                "Westfalen", "Brandenburg", "Bayern"
                ))

                nation = "Germany";
            
            if (Locationcheck(place, "Österreich","Austria")) nation = "Austria"; 
            if (Locationcheck(place, "Pommern")) nation = "Germany";
            if (Locationcheck(place, "Burma")) nation = "Burma";  
            if (Locationcheck(place, "Puerto Rico")) nation = "Puerto Rico"; 
            if (Locationcheck(place, "Austria")) nation = "Austria";
            if (Locationcheck(place, "Bavaria")) nation = "Germany";
            if (Locationcheck(place, "Honduras")) nation = "Honduras";
            if (Locationcheck(place, "Ireland")) nation = "Ireland";
            if (Locationcheck(place, "Scotland", "Scottland")) nation = "Scotland";
            if (Locationcheck(place, "Switzerland", "Switerland","switz")) nation = "Switzerland";
            if (Locationcheck(place, "New Zealand")) nation = "New Zealand";

            if (Locationcheck(place, "Denmark","Danmark")) nation = "Denmark";
            if (Locationcheck(place, "Luxembourg")) nation = "Luxembourg";
            if (Locationcheck(place, "Nicaragua")) nation = "Nicaragua";
            if (Locationcheck(place, "Philippines", "Philippine")) nation = "Philippines";
            if (Locationcheck(place, "Norway")) nation = "Norway";
            if (Locationcheck(place, "West Indies", "Barbados","Bermuda")) nation = "West Indies";
            if (Locationcheck(place, "South Africa")) nation = "South Africa";
            if (Locationcheck(place, "Canada","Newfoundland")) nation = "Canada"; 
            if (Locationcheck(place, "Québec", "Quebec")) nation = "Canada";
            if (Locationcheck(place, "Brazil", "Brasil")) nation = "Brasil";
            if (Locationcheck(place, "England", "United Kingdom", "Isle of Man", "Channel Islands", "eng."))
            {
                if (Locationcheck(place, "New England"))
                {
                    nation = "USA";
                }
                else
                {
                    if (Locationcheck(place, "USA", "United States"))
                    {
                        //the location string probably has garbage in it, ie USA or England etc
                        nation = "Unknown";
                    }
                    else
                    {
                        if (nation != "USA" && Locationcheck(place, "London"))
                            nation = "England";
                        else
                            nation = "England";
                    }
                  
                }
            }
            if (Locationcheck(place, "Sweden")) nation = "Sweden";
            if (Locationcheck(place, "Sverige")) nation = "Sweden";
            if (Locationcheck(place, "Holland")) nation = "Holland";
            if (Locationcheck(place, "Netherlands","Nederland")) nation = "Netherlands";
            if (Locationcheck(place, "Finland")) nation = "Finland";
            if (Locationcheck(place, "Czech", "Czeckoslavakia", "Prague")) nation = "Czech Republic";
            if (Locationcheck(place, "Yugoslavia")) nation = "Yugoslavia";
            if (Locationcheck(place, "Italy","Italia", "Sicily")) nation = "Italy";
            if (Locationcheck(place, "Wales")) nation = "Wales";
            if (Locationcheck(place, "Spain")) nation = "Spain";
            if (Locationcheck(place, "Estonia")) nation = "Estonia";
            if (Locationcheck(place, "Portugal")) nation = "Portugal";
            if (Locationcheck(place, "Poland")) nation = "Poland";
            if (Locationcheck(place, "Belgium")) nation = "Belgium";
            if (Locationcheck(place, "Lithuania")) nation = "Lithuania";
            if (Locationcheck(place, "Latvia")) nation = "Latvia";
            if (Locationcheck(place, "Romania")) nation = "Romania";
            if (Locationcheck(place, "Columbia")) nation = "Columbia";
            if (Locationcheck(place, "Slovakia")) nation = "Slovakia";
            if (Locationcheck(place, "Croatia")) nation = "Croatia";
            if (Locationcheck(place, "Bohemia")) nation = "Bohemia";
            if (Locationcheck(place, "Mexico")) nation = "Mexico";
            if (Locationcheck(place, "Azores")) nation = "Azores";
            if (Locationcheck(place, "Peru")) nation = "Peru";

            return nation;
        }


        public void Init()
        {
            var placeContext = new PlacesContext();

            foreach (var place in placeContext.Places)
            {
               

                if (place.Place15nm.Contains("\"") && place.Splitind.Contains("\"") && place.Fid == "")
                {
                    Console.WriteLine(place.Place15nm);

                    place.Place15nm = place.Place15nm + place.Splitind;
                    place.Splitind = place.Popcnt;
                    place.Popcnt = place.Descnm;
                    place.Descnm = place.Ctyhistnm;
                    place.Ctyhistnm = place.Ctyltnm;
                    place.Ctyltnm = place.Ctry15nm;
                    place.Ctry15nm = place.Cty15cd;
                    place.Cty15cd = place.Cty15nm;
                    place.Cty15nm = place.Lad15cd;
                    place.Lad15cd = place.Lad15nm;
                    place.Lad15nm = place.Lad15nm;
                    place.Laddescnm = place.Wd15cd;
                    place.Wd15cd = place.Par15cd;
                    place.Par15cd = place.Hlth12cd;
                    place.Hlth12cd = place.Hlth12nm;
                    place.Hlth12nm = place.Regd15cd;
                    place.Regd15cd = place.Regd15nm;
                    place.Regd15nm = place.Rgn15cd;
                    place.Rgn15cd = place.Rgn15nm;
                    place.Rgn15nm = place.Npark15cd;
                    place.Npark15cd = place.Npark15nm;
                    place.Npark15nm = place.Bua11cd;
                    place.Bua11cd = place.Pcon15cd;
                    place.Pcon15cd = place.Pcon15nm;
                    place.Pcon15nm = place.Eer15cd;
                    place.Eer15cd = place.Eer15nm;
                    place.Eer15nm = place.Pfa15cd;
                    place.Pfa15cd = place.Pfa15nm;
                    place.Pfa15nm = place.Gridgb1m;
                    place.Gridgb1m = place.Gridgb1e;
                    place.Gridgb1e = place.Gridgb1n;
                    place.Gridgb1n = place.Grid1km;
                    place.Grid1km = place.Lat;
                    place.Lat = place.Long;
                    place.Long = place.Fid;
                    place.Fid = "";
                }


                if (place.Fid == "" && place.Long == "" && place.Lat.Contains(','))
                {

                    var parts = place.Lat.Split(',');

                    if (parts.Length == 3)
                    {
                        place.Lat = parts[0];
                        place.Long = parts[1];
                        place.Fid = parts[2];
                    }
                }
            }
            Console.WriteLine("saving");
            placeContext.SaveChanges();

            Console.WriteLine("finished");
        }

        public static bool CheckUSA(string place)
        {

            place = place.Trim().ToLower();

            var mustnotContain = new List<string>() {
                "Canada",
                "Ireland",
                "England",
                "Durham",
                "Donegal"
            };

            if (place == null) return false;

            foreach (var test in mustnotContain)
            {
                if (place.Contains(test.ToLower())) return false;
            }

            var containsList = new List<string>() {
                "United States",
                "USA","Pennslyvania","penn.","Pennsy","Carolina"
            };

            var endsWithList = new List<string>() {
                "US"
            };


            foreach (var test in containsList)
            {
                if (place.Contains(test.ToLower())) return true;
            }

            foreach (var test in endsWithList)
            {
                if (place.EndsWith(test.ToLower())) return true;
            }

            foreach (var s in States.los)
            {
                if (place.Contains(s.Name.ToLower())) return true;
            }

            foreach (var s in States.los)
            {           

                if (place.Length == 2 && place == s.Abbreviation.ToLower())
                {
                    return true;
                }

                if (place.EndsWith("" + s.Abbreviation.ToLower() + ".")) return true;

                if (place.Contains(" " + s.Abbreviation.ToLower() + " ")) return true;

                if (place.Contains("," + s.Abbreviation.ToLower() + ",")) return true;

                if (place.EndsWith(" " + s.Abbreviation.ToLower() + "")) return true;

                if (place.EndsWith("/" + s.Abbreviation.ToLower() + "")) return true;

                if (place.EndsWith("." + s.Abbreviation.ToLower() + "")) return true;
            }

            if (place.Contains(" co. ")) return true;

            return false;
        }

        public static bool Locationcheck(string place, params string[] locations)
        {
            foreach (var test in locations)
            {
                if (place != null && place.ToLower().Contains(test.ToLower())) return true;
            }

            return false;
        }

    }
}
