using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class UserDefinedTag : BaseEntity
    {
        public string Tag { get; set; }
        public string Value { get; set; }
        public new IList<UserDefinedTag> UserDefinedTags { get; set; }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatUserDefinedTagString(hierarchyRoot));
            foreach (var tag in UserDefinedTags)
                output.Append(tag.ToGEDCOMString(hierarchyRoot + 1));
            return output.ToString();
        }

        private string FormatUserDefinedTagString(int hierarchyRoot)
        {
            if (!string.IsNullOrWhiteSpace(Value))
                return string.Format("{0} {1} {2}", hierarchyRoot, Tag, Value);
            else
                return string.Format("{0} {1}", hierarchyRoot, Tag);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<UserDefinedTag> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildUserDefinedTag(i, context)).ToList();
        }

        private static UserDefinedTag BuildUserDefinedTag(DataHierarchyItem userDefinedTag, GEDCOMContext context)
        {
            var result = new UserDefinedTag();
            result.Context = context;

            if (userDefinedTag.Value.Contains(' '))
            {
                result.Tag = userDefinedTag.Value.Split(' ')[0];
                result.Value = userDefinedTag.Value.Split(' ')[1];
            }
            else
            {
                result.Tag = userDefinedTag.Value.Split(' ')[0];
                result.Value = string.Empty;
            }
            result.UserDefinedTags = userDefinedTag.Items.Select(i => BuildUserDefinedTag(i, context)).ToList();

            return result;
        }
        #endregion
    }
}
