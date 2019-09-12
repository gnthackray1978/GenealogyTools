using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Label : BaseEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string ColorText { get; set; }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatLabelString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Title))
                output.AppendLine(FormatTitleString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(ColorText))
                output.AppendLine(FormatColorString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatLabelString(int hierarchyRoot)
        {
            return string.Format("{0} @L{1}@ LABL", hierarchyRoot, ID.ToString());
        }

        private string FormatTitleString(int hierarchyRoot)
        {
            return string.Format("{0} TITL {1}", hierarchyRoot, Title);
        }

        private string FormatColorString(int hierarchyRoot)
        {
            return string.Format("{0} COLR {1}", hierarchyRoot, ColorText);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Label> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildLabel(i, context)).ToList();
        }

        private static Label BuildLabel(DataHierarchyItem label, GEDCOMContext context)
        {
            var result = new Label();
            result.Context = context;

            var titleItems = label.Items.Where(i => i.Value.StartsWith("TITL"));
            var colorItems = label.Items.Where(i => i.Value.StartsWith("COLR"));
            var userDefinedItems = label.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = label.Value.GetID("L", 0);
            result.Title = titleItems.GetValue();
            result.ColorText = colorItems.GetValue();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
