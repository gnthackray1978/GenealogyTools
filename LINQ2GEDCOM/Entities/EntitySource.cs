using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class EntitySource : BaseEntity
    {
        public int ID { get; set; }
        public string QualityOfData { get; set; }

        public Source Source
        {
            get
            {
                return Context.Sources.Where(s => s.ID == ID).Single();
            }
        }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatSourceString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(QualityOfData))
                output.AppendLine(FormatQualityOfDataString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatSourceString(int hierarchyRoot)
        {
            return string.Format("{0} SOUR @S{1}@", hierarchyRoot, ID);
        }

        private string FormatQualityOfDataString(int hierarchyRoot)
        {
            return string.Format("{0} QUAY {1}", hierarchyRoot, QualityOfData);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<EntitySource> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildEntitySource(i, context)).ToList();
        }

        private static EntitySource BuildEntitySource(DataHierarchyItem source, GEDCOMContext context)
        {
            var result = new EntitySource();
            result.Context = context;

            var qualityOfDataItems = source.Items.Where(i => i.Value.StartsWith("QUAY"));
            var userDefinedItems = source.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = source.Value.GetID("S");
            result.QualityOfData = qualityOfDataItems.GetValue();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
