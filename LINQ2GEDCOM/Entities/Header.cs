using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Header : BaseEntity
    {
        public HeaderSource Source { get; set; }
        public string Character { get; set; }
        public Gedcom Gedcom { get; set; }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            
            output.AppendLine(FormatHeaderString(hierarchyRoot));
            
            if (Source != null)
                output.AppendLine(FormatSourceString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Character))
                output.AppendLine(FormatCharacterString(hierarchyRoot + 1));

            if (Gedcom != null)
                output.Append(FormatGedcomString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatHeaderString(int hierarchyRoot)
        {
            return string.Format("{0} HEAD", hierarchyRoot);
        }

        private string FormatSourceString(int hierarchyRoot)
        {
            return Source.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatCharacterString(int hierarchyRoot)
        {
            return string.Format("{0} CHAR {1}", hierarchyRoot, Character);
        }

        private string FormatGedcomString(int hierarchyRoot)
        {
            return Gedcom.ToGEDCOMString(hierarchyRoot);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Header> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildHeader(i, context)).ToList();
        }

        private static Header BuildHeader(DataHierarchyItem header, GEDCOMContext context)
        {
            var result = new Header();
            result.Context = context;

            var sourceItems = header.Items.Where(i => i.Value.StartsWith("SOUR"));
            var characterItems = header.Items.Where(i => i.Value.StartsWith("CHAR"));
            var gedcomItems = header.Items.Where(i => i.Value.StartsWith("GEDC"));
            var userDefinedItems = header.Items.Where(i => i.Value.StartsWith("_"));

            result.Source = HeaderSource.FromDataHierarchy(sourceItems, context).LastOrDefault();
            result.Character = characterItems.GetValue();
            result.Gedcom = Gedcom.FromDataHierarchy(gedcomItems, context).LastOrDefault();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
