using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM
{
    internal static class IEnumerableExtensions
    {
        internal static string GetValue(this IEnumerable<DataHierarchyItem> items, int substringLength = 5)
        {
            if (items.Count() < 1)
                return string.Empty;
            return items.Last().Value.GetSubstring(substringLength);
        }

        internal static IList<string> GetValues(this IEnumerable<DataHierarchyItem> items)
        {
            return items.Select(i => i.Value.GetSubstring(5)).ToList();
        }

        internal static int? GetID(this IEnumerable<DataHierarchyItem> items, string characterToReplace)
        {
            if (items.Count() < 1)
                return null;
            return int.Parse(items.Last().Value.Split(' ')[1].Replace("@", string.Empty).Replace(characterToReplace, string.Empty));
        }

        internal static IList<int> GetIDs(this IEnumerable<DataHierarchyItem> items, string characterToReplace)
        {
            var list = new List<int>();

            foreach (var v in items)
            {
                var tp = v.Value.Split(' ');

                if (tp.Length > 1)
                {
                    var second = tp[1];

                    second = second.Replace("@", string.Empty);

                    second = second.Replace(characterToReplace, string.Empty);

                    int secondInt = 0;

                    if (!Int32.TryParse(second, out secondInt))
                    {
                        Debug.WriteLine(second + " failed parse");
                    }


                }
            }

            return list;
            // return items.Select(i => int.Parse(i.Value.Split(' ')[1].Replace("@", string.Empty).Replace(characterToReplace, string.Empty))).ToList();
        }
    }

    internal static class StringExtensions
    {
        internal static string GetSubstring(this string value, int startIndex)
        {
            if (value.Length < startIndex)
                return string.Empty;
            else
                return value.Substring(startIndex);
        }

        internal static int GetID(this string value, string characterToReplace, int index = 1)
        {
            return int.Parse(value.Split(' ')[index].Replace("@", string.Empty).Replace(characterToReplace, string.Empty));
        }

        internal static IEnumerable<string> SplitIntoChunks(this string text, int chunkSize)
        {
            int offset = 0;
            while (offset < text.Length)
            {
                int size = Math.Min(chunkSize, text.Length - offset);
                yield return text.Substring(offset, size);
                offset += size;
            }
        }
    }
}
