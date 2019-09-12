using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Family : BaseEntity
    {
        public int ID { get; set; }
        public int? HusbandID { get; set; }
        public int? WifeID { get; set; }
        public IList<int> ObjectIDs { get; set; }
        public IList<EntitySource> Sources { get; set; }
        public DateTime Change { get; set; }
        public DateTime Create { get; set; }
        public IList<Child> Children { get; set; }
        public Event Marriage { get; set; }
        public Event Divorce { get; set; }

        public Individual Husband
        {
            get
            {
                if (HusbandID.HasValue)
                    return Context.Individuals.Where(i => i.ID == HusbandID).Single();
                return null;
            }
        }

        public Individual Wife
        {
            get
            {
                if (WifeID.HasValue)
                    return Context.Individuals.Where(i => i.ID == WifeID).Single();
                return null;
            }
        }

        public IEnumerable<Object> Objects
        {
            get
            {
                return Context.Objects.Where(o => ObjectIDs.Contains(o.ID));
            }
        }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatFamilyString(hierarchyRoot));

            if (Marriage != null)
                output.Append(FormatMarriageString(hierarchyRoot + 1));

            if (Divorce != null)
                output.Append(FormatDivorceString(hierarchyRoot + 1));

            if (HusbandID.HasValue)
                output.AppendLine(FormatHusbandString(hierarchyRoot + 1));

            if (WifeID.HasValue)
                output.AppendLine(FormatWifeString(hierarchyRoot + 1));

            if (Children != null)
                output.Append(FormatChildrenString(hierarchyRoot + 1));

            if (Sources != null)
                output.Append(FormatSourceString(hierarchyRoot + 1));

            if (ObjectIDs != null)
                output.Append(FormatObjectsString(hierarchyRoot + 1));

            if (Change != null)
                output.Append(FormatChangeString(hierarchyRoot + 1));

            if (Create != null)
                output.Append(FormatCreateString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatFamilyString(int hierarchyRoot)
        {
            return string.Format("{0} @F{1}@ FAM", hierarchyRoot, ID);
        }

        private string FormatHusbandString(int hierarchyRoot)
        {
            return string.Format("{0} HUSB @I{1}@", hierarchyRoot, HusbandID);
        }

        private string FormatWifeString(int hierarchyRoot)
        {
            return string.Format("{0} WIFE @I{1}@", hierarchyRoot, WifeID);
        }

        private string FormatObjectsString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var objectID in ObjectIDs)
                output.AppendLine(string.Format("{0} OBJE @O{1}@", hierarchyRoot, objectID));
            return output.ToString();
        }

        private string FormatSourceString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var source in Sources)
                output.Append(source.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatChangeString(int hierarchyRoot)
        {
            return Change.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatCreateString(int hierarchyRoot)
        {
            return Create.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatChildrenString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var child in Children)
                output.Append(child.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatMarriageString(int hierarchyRoot)
        {
            return Marriage.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatDivorceString(int hierarchyRoot)
        {
            return Divorce.ToGEDCOMString(hierarchyRoot);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Family> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildFamily(i, context)).ToList();
        }

        private static Family BuildFamily(DataHierarchyItem family, GEDCOMContext context)
        {
            var result = new Family();
            result.Context = context;

            var husbandItems = family.Items.Where(i => i.Value.StartsWith("HUSB"));
            var wifeItems = family.Items.Where(i => i.Value.StartsWith("WIFE"));
            var sourceItems = family.Items.Where(i => i.Value.StartsWith("SOUR"));
            var objectItems = family.Items.Where(i => i.Value.StartsWith("OBJE"));
            var changeItems = family.Items.Where(i => i.Value.StartsWith("CHAN"));
            var createItems = family.Items.Where(i => i.Value.StartsWith("CREA"));
            var childItems = family.Items.Where(i => i.Value.StartsWith("CHIL"));
            var marriageItems = family.Items.Where(i => i.Value.StartsWith("MARR"));
            var divorceItems = family.Items.Where(i => i.Value.StartsWith("DIV"));
            var userDefinedItems = family.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = family.Value.GetID("F", 0);
            result.HusbandID = husbandItems.GetID("I");
            result.WifeID = wifeItems.GetID("I");
            result.ObjectIDs = objectItems.GetIDs("O");
            result.Sources = EntitySource.FromDataHierarchy(sourceItems, context);
            result.Change = DateTime.FromDataHierarchy(changeItems, context, DateTime.DateType.Change).LastOrDefault();
            result.Create = DateTime.FromDataHierarchy(createItems, context, DateTime.DateType.Create).LastOrDefault();
            result.Children = Child.FromDataHierarchy(childItems, context);
            result.Marriage = Event.FromDataHierarchy(marriageItems, context, Event.EventType.Marriage).LastOrDefault();
            result.Divorce = Event.FromDataHierarchy(divorceItems, context, Event.EventType.Divorce).LastOrDefault();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
