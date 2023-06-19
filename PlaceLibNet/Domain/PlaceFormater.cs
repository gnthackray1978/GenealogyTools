using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConfigHelper;
using PlaceLibNet.Data.Contexts;

namespace PlaceLibNet.Domain
{
    public class PlaceNameFormatter : IPlaceNameFormatter
    {
        public string Format(string place)
        {
            place = place.Replace(" ", "");

            place = RemoveCommasAndPipes(place);

            place = ReplaceSlashesWithSingleSlash(place);

            place = DeleteNonAlphaNumericExceptSlash(place);

            return place;
        }

        /// <summary>
        /// Make location name lowercase and remove anything that's not a letter
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string FormatComponent(string input)
        {
            input = input.ToLower();

            var regex = new Regex(@"[^a-z]");

            
            return regex.Replace(input, "");
        }

        private string RemoveCommasAndPipes(string input)
        {
            input = input.Replace(',', '/');
            input = input.Replace('|', ' ').Trim();

            return input;
        }

        private string ReplaceSlashesWithSingleSlash(string input)
        {
            // Create a regular expression that matches a single slash or any number of whitespace characters.
            var regex = new Regex(@"(\s+/\s+|\s+/|/\s+)");

            // Replace all matches of the regular expression with a single slash.
            return regex.Replace(input, "/");
        }

        private string DeleteNonAlphaNumericExceptSlash(string input)
        {
            // Create a regular expression that matches a single slash or any number of whitespace characters.
            var regex = new Regex(@"[^a-zA-Z\d\s/]");

            // Replace all matches of the regular expression with a single slash.
            return regex.Replace(input, "");
        }
    }
}
