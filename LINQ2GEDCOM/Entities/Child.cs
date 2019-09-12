using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Child : BaseEntity
    {
        public int ID { get; set; }
        public string Pedigree { get; set; }

        public Individual Individual
        {
            get
            {
                return Context.Individuals.Where(i => i.ID == ID).Single();
            }
        }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatChildString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Pedigree))
                output.AppendLine(FormatPedigreeString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatChildString(int hierarchyRoot)
        {
            return string.Format("{0} CHIL @I{1}@", hierarchyRoot, ID);
        }

        private string FormatPedigreeString(int hierarchyRoot)
        {
            return string.Format("{0} PEDI {1}", hierarchyRoot, Pedigree);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Child> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildChild(i, context)).ToList();
        }

        private static Child BuildChild(DataHierarchyItem child, GEDCOMContext context)
        {
            var result = new Child();
            result.Context = context;

            var pedigreeItems = child.Items.Where(i => i.Value.StartsWith("PEDI"));
            var userDefinedItems = child.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = child.Value.GetID("I");
            result.Pedigree = pedigreeItems.GetValue();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
