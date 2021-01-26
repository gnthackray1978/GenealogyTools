using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FTMContext.lib
{
    public class GoogleGeoCodingHelpers
    {
        /// <summary>
        /// format cache place entry prior to it being looked up by geolocator.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FormatPlace(string name)
        {
            if (name == null) return "";

            var splits = name.Split('/');

            var newPlace = "";

            foreach (var s in splits)
            {

                var errorCounter = Regex.Matches(s, @"[a-zA-Z]").Count;

                if (errorCounter > 0)
                {
                    newPlace += "/" + s;
                }

            }

            return newPlace;
        }

        public static string GetType(List<Result> _root, string type, string subType)
        {
            foreach (var result in _root)
            {
                foreach (var ac in result.address_components)
                {

                    bool isMissing = false;

                    foreach (var comp in ac.types)
                    {
                        if (type != "administrative_area_level_2")
                        {
                            if (comp == type)
                            {
                                return ac.long_name;
                            }
                        }
                        else
                        {
                            if (comp == type)
                            {

                                if (subType == "England")
                                {
                                    if (HistoricCounties.Get.Contains(ac.long_name))
                                    {
                                        return ac.long_name;
                                    }
                                    else
                                    {

                                        if (ac.long_name.Contains("Yorkshire"))
                                        {
                                            return "Yorkshire";
                                        }

                                        if (ac.long_name.Contains("Cumbria"))
                                        {
                                            return "Cumbria";
                                        }

                                        if (ac.long_name.Contains("Chesire"))
                                        {
                                            return "Chesire";
                                        }

                                        if (ac.long_name.Contains("Sussex"))
                                        {
                                            return "Sussex";
                                        }

                                        if (ac.long_name.Contains("Durham"))
                                        {
                                            return "Durham";
                                        }

                                        if (ac.long_name.Contains("London"))
                                        {
                                            return "London";
                                        }

                                        if (ac.long_name.Contains("Peterborough"))
                                        {
                                            return "Northamptonshire";
                                        }
                                    }
                                }
                                else
                                {
                                    return ac.long_name;
                                }
                            }
                        }
                    }
                }
            }

            return "";
        }

    }
}
