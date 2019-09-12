using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM
{
    internal static class HelperExtensions
    {
        internal static string SubstringOrDefault(this string str, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentException("Start index must be a non-negative integer.");

            if (str.Length < startIndex)
                return string.Empty;
            return str.Substring(startIndex);
        }

        // This came from SLaks: http://stackoverflow.com/questions/3008718/split-string-into-smaller-strings-by-length-variable/3008775#3008775
        // But I split it up into two methods to add input checking, see C# In Depth 2nd Edition Chapter 6
        internal static IEnumerable<string> SplitByLength(this string str, int segmentLength)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("String must not be empty.");
            if (segmentLength < 1)
                throw new ArgumentException("Segment length must be a positive integer.");

            return SplitByLengthImpl(str, segmentLength);
        }

        private static IEnumerable<string> SplitByLengthImpl(string str, int segmentLength)
        {
            for (var i = 0; i < str.Length; i += segmentLength)
                yield return str.Substring(i, Math.Min(segmentLength, str.Length - i));
        }
    }
}
