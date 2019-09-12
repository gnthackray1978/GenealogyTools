using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class DateTime : BaseEntity
    {
        private DateType _dateType;

        public string Date { get; set; }
        public string Time { get; set; }

        public System.DateTime? DateTimeValue
        {
            get
            {
                var result = System.DateTime.MinValue;
                if (System.DateTime.TryParse(string.Format("{0} {1}", Date, Time), out result))
                    return result;
                return null;
            }
        }

        private string DateString
        {
            get
            {
                switch (_dateType)
                {
                    case DateType.Change:
                        return "CHAN";
                    case DateType.Create:
                        return "CREA";
                    default:
                        throw new Exception("Invalid Date Type");
                }
            }
        }

        public enum DateType
        {
            Change,
            Create
        }

        private DateTime() { }

        public DateTime(DateType dateType) : this()
        {
            _dateType = dateType;
        }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatDateTimeString(hierarchyRoot));

            output.AppendLine(FormatDateString(hierarchyRoot + 1).Trim());

            if (!string.IsNullOrWhiteSpace(Time))
                output.AppendLine(FormatTimeString(hierarchyRoot + 2));

            return output.ToString();
        }

        private string FormatDateTimeString(int hierarchyRoot)
        {
            return string.Format("{0} {1}", hierarchyRoot, DateString);
        }

        private string FormatDateString(int hierarchyRoot)
        {
            return string.Format("{0} DATE {1}", hierarchyRoot, Date);
        }

        private string FormatTimeString(int hierarchyRoot)
        {
            return string.Format("{0} TIME {1}", hierarchyRoot, Time);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<DateTime> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context, DateType dateType)
        {
            return items.Select(i => BuildDateTime(i, context, dateType)).ToList();
        }

        private static LINQ2GEDCOM.Entities.DateTime BuildDateTime(DataHierarchyItem dateTime, GEDCOMContext context, DateType dateType)
        {
            var result = new LINQ2GEDCOM.Entities.DateTime(dateType);
            result.Context = context;

            var changeDateItems = dateTime.Items.Where(i => i.Value.StartsWith("DATE"));
            var userDefinedItems = dateTime.Items.Where(i => i.Value.StartsWith("_"));

            foreach (var changeDateItem in changeDateItems)
            {
                result.Date = changeDateItem.Value.GetSubstring(5);
                result.Time = changeDateItem.Items.Where(i => i.Value.StartsWith("TIME")).GetValue();
                result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);
            }

            return result;
        }
        #endregion
    }
}
