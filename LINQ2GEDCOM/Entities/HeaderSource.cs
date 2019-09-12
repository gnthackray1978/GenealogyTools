using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class HeaderSource : BaseEntity
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Corporate { get; set; }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatSourceString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Name))
                output.AppendLine(FormatNameString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Version))
                output.AppendLine(FormatVersionString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Corporate))
                output.Append(FormatCorporateString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatSourceString(int hierarchyRoot)
        {
            return string.Format("{0} SOUR", hierarchyRoot);
        }

        private string FormatNameString(int hierarchyRoot)
        {
            return string.Format("{0} NAME {1}", hierarchyRoot, Name);
        }

        private string FormatVersionString(int hierarchyRoot)
        {
            return string.Format("{0} VERS {1}", hierarchyRoot, Version);
        }

        private string FormatCorporateString(int hierarchyRoot)
        {
            return string.Format("{0} CORP {1}", hierarchyRoot, Corporate);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<HeaderSource> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildHeaderSource(i, context)).ToList();
        }

        private static HeaderSource BuildHeaderSource(DataHierarchyItem source, GEDCOMContext context)
        {
            var result = new HeaderSource();
            result.Context = context;

            var nameItems = source.Items.Where(i => i.Value.StartsWith("NAME"));
            var versionItems = source.Items.Where(i => i.Value.StartsWith("VERS"));
            var corporateItems = source.Items.Where(i => i.Value.StartsWith("CORP"));
            var userDefinedItems = source.Items.Where(i => i.Value.StartsWith("_"));

            result.Name = nameItems.GetValue();
            result.Version = versionItems.GetValue();
            result.Corporate = corporateItems.GetValue();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
