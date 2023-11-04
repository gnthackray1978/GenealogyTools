using System;

namespace FTMContextNet.Domain.ExtensionMethods
{
    public static class StringExtender
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsSpecified(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static double ToDouble(this string value)
        {
            if(Double.TryParse(value, out double result))
                return result;

            return 0.0;
        }

        public static decimal ToDecimal(this string value)
        {
            if (decimal.TryParse(value, out decimal result))
                return result;

            return 0;
        }
    }
}