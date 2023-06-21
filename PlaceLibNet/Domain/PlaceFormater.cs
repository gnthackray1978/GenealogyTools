using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlaceLibNet.Domain
{
    public class PlaceNameFormatter : IPlaceNameFormatter
    {
        /// <summary>
        /// Replace blocks of white space with single space
        /// Remove pipes dupe slashes and non alpa numeric characters except slashes
        /// Remove leading slashes
        /// </summary>
        public string Format(string place)
        {
            place = place.Replace("  ", " ");

            place = place.Replace("-", " ");

            place = place.TrimStart(',');

            place = RemoveCommasAndPipes(place);

            place = DeleteNonAlphaNumericExceptSlash(place);

            place = ReplaceSlashesWithSingleSlash(place);

            place = place.Trim().Trim('/');

            place = place.Replace("/ ", "/");

            return place;
        }
        
        /// <summary>
        /// Valid when has 3 components AND
        /// is in England or Wales
        /// </summary>
        public bool IsValidEnglandWales(string place, char placeMarker = '/')
        {
            var count = place.Count(c => c == placeMarker);

            if (place.Contains("england", StringComparison.OrdinalIgnoreCase) 
                || place.Contains("wales", StringComparison.OrdinalIgnoreCase))
            {
                if (count > 1)
                {
                    return true;
                }
            }

            return false;
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

        /// <summary>
        /// Replace commas with slashes
        /// Replace pipes with white space
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveCommasAndPipes(string input)
        {
            input = input.Replace(',', '/');
            input = input.Replace('|', ' ').Trim();

            return input;
        }

        private string ReplaceSlashesWithSingleSlash(string input)
        {
            // Create a regular expression that matches a single slash or any number of whitespace characters.
            var regex = new Regex(@"/+");

            // Replace all matches of the regular expression with a single slash.
            return regex.Replace(input, "/");
        }

        private string DeleteNonAlphaNumericExceptSlash(string input)
        {
            // Deletes non alpha characters except slashes
            var regex = new Regex(@"[^a-zA-Z/ ]");

            // Replace all matches of the regular expression with a single slash.
            return regex.Replace(input, "");
        }
    }
}
