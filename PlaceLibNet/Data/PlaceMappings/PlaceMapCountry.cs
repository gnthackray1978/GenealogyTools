using System.Collections.Generic;
using PlaceLib;

namespace PlaceLibNet.Data.PlaceMappings
{
    public class PlaceMapCountry
    {
        public static string Find(string place)
        {

            var nation = "Unknown";



            if (Locationcheck(place, "Australia", "Queensland", "Tasmania")) nation = "Australia";
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

            if (Locationcheck(place, "Österreich", "Austria")) nation = "Austria";
            if (Locationcheck(place, "Pommern")) nation = "Germany";
            if (Locationcheck(place, "Burma")) nation = "Burma";
            if (Locationcheck(place, "Puerto Rico")) nation = "Puerto Rico";
            if (Locationcheck(place, "Austria")) nation = "Austria";
            if (Locationcheck(place, "Bavaria")) nation = "Germany";
            if (Locationcheck(place, "Honduras")) nation = "Honduras";
            if (Locationcheck(place, "Ireland")) nation = "Ireland";
            if (Locationcheck(place, "Scotland", "Scottland")) nation = "Scotland";
            if (Locationcheck(place, "Switzerland", "Switerland", "switz")) nation = "Switzerland";
            if (Locationcheck(place, "New Zealand")) nation = "New Zealand";

            if (Locationcheck(place, "Denmark", "Danmark")) nation = "Denmark";
            if (Locationcheck(place, "Luxembourg")) nation = "Luxembourg";
            if (Locationcheck(place, "Nicaragua")) nation = "Nicaragua";
            if (Locationcheck(place, "Philippines", "Philippine")) nation = "Philippines";
            if (Locationcheck(place, "Norway")) nation = "Norway";
            if (Locationcheck(place, "West Indies", "Barbados", "Bermuda")) nation = "West Indies";
            if (Locationcheck(place, "South Africa")) nation = "South Africa";
            if (Locationcheck(place, "Canada", "Newfoundland")) nation = "Canada";
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
                        //the location string probably has garbage in it, ie 'USA or England' etc
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
            if (Locationcheck(place, "Netherlands", "Nederland")) nation = "Netherlands";
            if (Locationcheck(place, "Finland")) nation = "Finland";
            if (Locationcheck(place, "Czech", "Czeckoslavakia", "Prague")) nation = "Czech Republic";
            if (Locationcheck(place, "Yugoslavia")) nation = "Yugoslavia";
            if (Locationcheck(place, "Italy", "Italia", "Sicily")) nation = "Italy";
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

        private static bool CheckUSA(string place)
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

        private static bool Locationcheck(string place, params string[] locations)
        {
            foreach (var test in locations)
            {
                if (place != null && place.ToLower().Contains(test.ToLower())) return true;
            }

            return false;
        }
    }
}
