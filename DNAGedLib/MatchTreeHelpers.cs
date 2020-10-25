﻿using System;
using System.Text.RegularExpressions;

namespace DNAGedLib
{
    public struct DateObj
    {
        public int YearInt { get; set; }
        public string DateStr { get; set; }
        public string Place { get; set; }
    }

    public class MatchTreeHelpers
    {
      

        public static DateTime ExtractDate(object originalText)
        {
            DateTime dt = DateTime.Today;

            if (originalText == null) return dt;

            var parts = originalText.ToString().Split('/');

            int day = 0;
            int month = 0;
            int year = 0;

            day = Convert.ToInt32(parts[1]);
            month = Convert.ToInt32(parts[0]);
            year = Convert.ToInt32(parts[2]);

            dt = new DateTime(year, month, day);

            return dt;

        }

        public static int ExtractYear(object originalText)
        {

            if (originalText == null) return 0;

            var parsedText = originalText.ToString();
            
            Regex regex = new Regex(@"\d\d\d\d");
            var v = regex.Match(parsedText);
            string anyoString = v.Groups[0].ToString();

            if (anyoString != String.Empty)
                return Convert.ToInt32(anyoString);
            else
                return 0;

        }

        public static double ExtractDouble(object originalText)
        {
            double retVal = 0;

            if (originalText == null) return retVal;

            var parsedText = originalText.ToString();



            Double.TryParse(parsedText, out retVal);


            return retVal;
        }

        public static int ExtractInt(object originalText)
        {
            int retVal = 0;

            if (originalText == null) return retVal;

            var parsedText = originalText.ToString();

            

            Int32.TryParse(parsedText, out retVal);


            return retVal;
        }

        public static long ExtractLong(object originalText)
        {
            long retVal = 0;

            if (originalText == null) return retVal;

            var parsedText = originalText.ToString();



            Int64.TryParse(parsedText, out retVal);


            return retVal;
        }

        public static bool ExtractBool(object originalText)
        {
            bool retVal = false;

            if (originalText == null) return false;

            var parsedText = originalText.ToString();



            Boolean.TryParse(parsedText, out retVal);


            return retVal;
        }

        public static Guid ExtractGuid(object originalText)
        {
            Guid retVal = Guid.Empty;

            if (originalText == null) return retVal;

            var parsedText = originalText.ToString();



            Guid.TryParse(parsedText, out retVal);


            return retVal;
        }
    }
}
