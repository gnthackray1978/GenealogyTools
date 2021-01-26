using PlaceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSVAnalyser
{
    public static class Helpers
    {
        //
         
        public static bool searchPlaces(string searchString)
        {
            List<string> ignoreList = new List<string>()
            {
                "new york",
                "charleston",
                "londonderry",
                "county",
                "township",
                "preston city",
                "melbourne"
            };

            searchString = searchString.ToLower().Trim().Replace("\"", "");

            //if (ignoreList.Contains(searchString)) return false;

            bool isFound = false;

            foreach (var s in ignoreList)
            {
                isFound = searchString.Contains(s);

                if (isFound)
                    return false;
            }


            isFound = false;

            foreach (var s in YorkshirePlaces.Get())
            {
                isFound = searchString.Contains(s);

                if (isFound)
                    break;
            }

            return isFound;

        }

           

        public static bool Intersection(this string str, string testVal)
        {
            testVal = testVal.Trim().ToLower();

            str = str.Trim().ToLower();

            return str.Contains(testVal);
        }

        //public static bool Exists(this List<Person> persons, Person testVal)
        //{
        //    return persons.Any(
        //        p =>
        //            p.UserName == testVal.UserName &&
        //            p.FirstName == testVal.FirstName &&
        //            p.LastName == testVal.LastName &&
        //            p.BirthInt == testVal.BirthInt);
        //}

        public static string Cleanup(this string str)
        {
            int maxlength = 254;
            str = str.Replace("\"", "");

            if (str.Length > maxlength)
            {
                str = str.Substring(0, 250);
            }

            return str;
        }


       

    }
}
