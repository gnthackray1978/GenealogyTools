using System;
using System.Text.RegularExpressions;
using TinyCsvParser.TypeConverter;

namespace CSVAnalyser
{
    public class Mytypeconv : ITypeConverter<int>
    {
   
 

        public Type TargetType
        {
            get { return typeof(int); }
        }

        public bool TryConvert(string value, out int result)
        {
            
            var r = new Regex(@"\b\d{4}\b");

            foreach (var match in r.Matches(value))
            {
                var date = 0;
                if (int.TryParse(match.ToString(), out date))
                {
                    result = date;
                    return true;
                }
            }

            result = 0;
            return true;

        }
    }
}