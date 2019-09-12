using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Gedcom : BaseEntity
    {
        public string Version { get; set; }
        public string Format { get; set; }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatGedcomString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Version))
                output.AppendLine(FormatVersionString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Format))
                output.AppendLine(FormatFormatString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatGedcomString(int hierarchyRoot)
        {
            return string.Format("{0} GEDC", hierarchyRoot);
        }

        private string FormatVersionString(int hierarchyRoot)
        {
            return string.Format("{0} VERS {1}", hierarchyRoot, Version);
        }

        private string FormatFormatString(int hierarchyRoot)
        {
            return string.Format("{0} FORM {1}", hierarchyRoot, Format);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Gedcom> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildGedcom(i, context)).ToList();
        }

        private static Gedcom BuildGedcom(DataHierarchyItem gedcom, GEDCOMContext context)
        {
            var result = new Gedcom();
            result.Context = context;

            var versionItems = gedcom.Items.Where(i => i.Value.StartsWith("VERS"));
            var formatItems = gedcom.Items.Where(i => i.Value.StartsWith("FORM"));
            var userDefinedItems = gedcom.Items.Where(i => i.Value.StartsWith("_"));

            result.Version = versionItems.GetValue();
            result.Format = formatItems.GetValue();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
