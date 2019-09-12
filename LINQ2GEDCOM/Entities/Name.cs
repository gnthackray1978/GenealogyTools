using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Name : BaseEntity
    {
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string SecondGivenName { get; set; }
        public string SurnamePrefix { get; set; }
        public string NamePrefix { get; set; }
        public string NameSuffix { get; set; }
        public string Type { get; set; }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatNameString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Type))
                output.AppendLine(FormatTypeString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(GivenName))
                output.AppendLine(FormatGivenNameString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Surname))
                output.AppendLine(FormatSurnameString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(SecondGivenName))
                output.AppendLine(FormatSecondGivenNameString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(SurnamePrefix))
                output.AppendLine(FormatSurnamePrefixString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(NamePrefix))
                output.AppendLine(FormatNamePrefixString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(NameSuffix))
                output.AppendLine(FormatNameSuffixString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatNameString(int hierarchyRoot)
        {
            var name = string.Empty;
            if (!string.IsNullOrWhiteSpace(GivenName))
                name = GivenName;
            if (!string.IsNullOrWhiteSpace(SecondGivenName))
                name = string.Format("{0} {1}", name, SecondGivenName);
            if (!string.IsNullOrWhiteSpace(SurnamePrefix))
                name = string.Format("{0} {1}", name, SurnamePrefix);
            name = string.Format("{0}/{1}/", name, Surname);

            return string.Format("{0} NAME {1}", hierarchyRoot, name);
        }

        private string FormatSurnameString(int hierarchyRoot)
        {
            return string.Format("{0} SURN {1}", hierarchyRoot, Surname);
        }

        private string FormatGivenNameString(int hierarchyRoot)
        {
            return string.Format("{0} GIVN {1}", hierarchyRoot, GivenName);
        }

        private string FormatSecondGivenNameString(int hierarchyRoot)
        {
            return string.Format("{0} SECG {1}", hierarchyRoot, SecondGivenName);
        }

        private string FormatSurnamePrefixString(int hierarchyRoot)
        {
            return string.Format("{0} SPFX {1}", hierarchyRoot, SurnamePrefix);
        }

        private string FormatNamePrefixString(int hierarchyRoot)
        {
            return string.Format("{0} NPFX {1}", hierarchyRoot, NamePrefix);
        }

        private string FormatNameSuffixString(int hierarchyRoot)
        {
            return string.Format("{0} NSFX {1}", hierarchyRoot, NameSuffix);
        }

        private string FormatTypeString(int hierarchyRoot)
        {
            return string.Format("{0} TYPE {1}", hierarchyRoot, Type);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Name> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildName(i, context)).ToList();
        }

        private static Name BuildName(DataHierarchyItem name, GEDCOMContext context)
        {
            var result = new Name();
            result.Context = context;

       //     if (name.Items.Count == 0)
           // {
                var tp = name.Value.Replace("NAME", "").Trim();

                var parts = tp.Split('/');

                

                if (parts.Length == 1)
                {
                    result.Surname = parts[0];
                }
                else
                {

                   

                    if (parts.Length > 1)
                    {
                        if (parts.Last() == "")
                            parts = parts.Where(x => x != "").ToArray();


                        result.Surname = parts.Last().Trim();

                        tp = tp.Replace(parts.Last(), "");
                        tp = tp.Replace('/', ' ');

                        result.GivenName = tp.Trim();

                    }
                }

           //     return result;
        //    }

            var typeItems = name.Items.Where(i => i.Value.StartsWith("TYPE"));
            var givenNameItems = name.Items.Where(i => i.Value.StartsWith("GIVN"));
            var secondGivenNameItems = name.Items.Where(i => i.Value.StartsWith("SECG"));
            var surnameItems = name.Items.Where(i => i.Value.StartsWith("SURN"));
            var surnamePrefixItems = name.Items.Where(i => i.Value.StartsWith("SPFX"));
            var namePrefixItems = name.Items.Where(i => i.Value.StartsWith("NPFX"));
            var nameSuffixItems = name.Items.Where(i => i.Value.StartsWith("NSFX"));
            var userDefinedItems = name.Items.Where(i => i.Value.StartsWith("_"));

            result.Type = typeItems.GetValue();
          //  result.GivenName = givenNameItems.GetValue();
          //  result.SecondGivenName = secondGivenNameItems.GetValue();
           // result.Surname = surnameItems.GetValue();
            result.SurnamePrefix = surnamePrefixItems.GetValue();
            result.NamePrefix = namePrefixItems.GetValue();
            result.NameSuffix = nameSuffixItems.GetValue();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);



            return result;
        }
        #endregion
    }
}
