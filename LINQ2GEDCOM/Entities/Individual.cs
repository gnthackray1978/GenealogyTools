using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Individual : BaseEntity
    {
        public int ID { get; set; }
        public IList<Name> Names { get; set; }
        public string Sex { get; set; }
        public IList<EntitySource> Sources { get; set; }
        public IList<int> NoteIDs { get; set; }
        public IList<int> ObjectIDs { get; set; }
        public DateTime Change { get; set; }
        public DateTime Create { get; set; }
        public IList<int> SpousalFamilyIDs { get; set; }
        public int? ParentFamilyID { get; set; }
        public Event Burial { get; set; }
        public Event Death { get; set; }
        public Event Birth { get; set; }
        public Event Adoption { get; set; }
        public IList<Event> Immigrations { get; set; }
        public IList<Event> Emigrations { get; set; }
        public IList<Event> MilitaryServices { get; set; }
        public IList<Event> Ordinances { get; set; }
        public IList<Event> Ordinations { get; set; }
        public IList<Event> Naturalizations { get; set; }

        public IEnumerable<Note> Notes
        {
            get
            {
                return Context.Notes.Where(n => NoteIDs.Contains(n.ID));
            }
        }

        public IEnumerable<Object> Objects
        {
            get
            {
                return Context.Objects.Where(o => ObjectIDs.Contains(o.ID));
            }
        }

        public IEnumerable<Family> SpousalFamilies
        {
            get
            {
                return Context.Families.Where(f => SpousalFamilyIDs.Contains(f.ID));
            }
        }

        public Family ParentFamily
        {
            get
            {
                if (ParentFamilyID.HasValue)
                    return Context.Families.Where(f => f.ID == ParentFamilyID).Single();
                return null;
            }
        }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatIndividualString(hierarchyRoot));

            if (Names != null)
                if (Names.Count > 0)
                    output.Append(FormatPrimaryNameString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Sex))
                output.AppendLine(FormatSexString(hierarchyRoot + 1));

            if (Names != null)
                if (Names.Count > 1)
                    output.Append(FormatAdditionalNamesString(hierarchyRoot + 1));

            if (Birth != null)
                output.Append(FormatBirthString(hierarchyRoot + 1));

            if (Adoption != null)
                output.Append(FormatAdoptionString(hierarchyRoot + 1));

            if (Immigrations != null)
                output.Append(FormatImmigrationString(hierarchyRoot + 1));

            if (Naturalizations != null)
                output.Append(FormatNaturalizationString(hierarchyRoot + 1));

            if (Emigrations != null)
                output.Append(FormatEmigrationString(hierarchyRoot + 1));

            if (MilitaryServices != null)
                output.Append(FormatMilitaryServiceString(hierarchyRoot + 1));

            if (Ordinances != null)
                output.Append(FormatOrdinanceString(hierarchyRoot + 1));

            if (Ordinations != null)
                output.Append(FormatOrdinationString(hierarchyRoot + 1));

            if (Death != null)
                output.Append(FormatDeathString(hierarchyRoot + 1));

            if (Burial != null)
                output.Append(FormatBurialString(hierarchyRoot + 1));

            if (Sources != null)
                output.Append(FormatSourceString(hierarchyRoot + 1));

            if (NoteIDs != null)
                output.Append(FormatNoteString(hierarchyRoot + 1));

            if (ObjectIDs != null)
                output.Append(FormatObjectsString(hierarchyRoot + 1));

            if (Change != null)
                output.Append(FormatChangeString(hierarchyRoot + 1));

            if (Create != null)
                output.Append(FormatCreateString(hierarchyRoot + 1));

            if (SpousalFamilyIDs != null)
                output.Append(FormatFamilySpouseString(hierarchyRoot + 1));

            if (ParentFamilyID.HasValue)
                output.AppendLine(FormatFamilyChildString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatIndividualString(int hierarchyRoot)
        {
            return string.Format("{0} @I{1}@ INDI", hierarchyRoot, ID);
        }

        private string FormatPrimaryNameString(int hierarchyRoot)
        {
            return string.Format(Names.First().ToGEDCOMString(hierarchyRoot));
        }

        private string FormatAdditionalNamesString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var name in Names.Skip(1))
                output.Append(name.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatSexString(int hierarchyRoot)
        {
            return string.Format("{0} SEX {1}", hierarchyRoot, Sex);
        }

        private string FormatSourceString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var source in Sources)
                output.Append(source.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatImmigrationString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var immigration in Immigrations)
                output.Append(immigration.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatNaturalizationString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var naturalization in Naturalizations)
                output.Append(naturalization.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatEmigrationString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var emigration in Emigrations)
                output.Append(emigration.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatMilitaryServiceString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var service in MilitaryServices)
                output.Append(service.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatOrdinanceString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var ordinance in Ordinances)
                output.Append(ordinance.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatOrdinationString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var ordination in Ordinations)
                output.Append(ordination.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatNoteString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var noteID in NoteIDs)
                output.AppendLine(string.Format("{0} NOTE @N{1}@", hierarchyRoot, noteID));
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

        private string FormatFamilySpouseString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var spousalFamilyID in SpousalFamilyIDs)
                output.AppendLine(string.Format("{0} FAMS @F{1}@", hierarchyRoot, spousalFamilyID));
            return output.ToString();
        }

        private string FormatObjectsString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var objectID in ObjectIDs)
                output.AppendLine(string.Format("{0} OBJE @O{1}@", hierarchyRoot, objectID));
            return output.ToString();
        }

        private string FormatFamilyChildString(int hierarchyRoot)
        {
            return string.Format("{0} FAMC @F{1}@", hierarchyRoot, ParentFamilyID);
        }

        private string FormatBurialString(int hierarchyRoot)
        {
            return Burial.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatDeathString(int hierarchyRoot)
        {
            return Death.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatBirthString(int hierarchyRoot)
        {
            return Birth.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatAdoptionString(int hierarchyRoot)
        {
            return Adoption.ToGEDCOMString(hierarchyRoot);
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Individual> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildIndividual(i, context)).ToList();
        }

        private static Individual BuildIndividual(DataHierarchyItem individual, GEDCOMContext context)
        {
            var result = new Individual();
            result.Context = context;

            var nameItems = individual.Items.Where(i => i.Value.StartsWith("NAME"));
            var sexItems = individual.Items.Where(i => i.Value.StartsWith("SEX"));
            var burialItems = individual.Items.Where(i => i.Value.StartsWith("BURI"));
            var deathItems = individual.Items.Where(i => i.Value.StartsWith("DEAT"));
            var birthItems = individual.Items.Where(i => i.Value.StartsWith("BIRT"));
            var adoptionItems = individual.Items.Where(i => i.Value.StartsWith("ADOP"));
            var immigrationItems = individual.Items.Where(i => i.Value.StartsWith("IMMI"));
            var naturalizationItems = individual.Items.Where(i => i.Value.StartsWith("NATU"));
            var emigrationItems = individual.Items.Where(i => i.Value.StartsWith("EMIG"));
            var militaryServiceItems = individual.Items.Where(i => i.Value.StartsWith("MISE"));
            var ordinanceItems = individual.Items.Where(i => i.Value.StartsWith("ORDI"));
            var ordinationItems = individual.Items.Where(i => i.Value.StartsWith("ORDN"));
            var sourceItems = individual.Items.Where(i => i.Value.StartsWith("SOUR"));
            var noteItems = individual.Items.Where(i => i.Value.StartsWith("NOTE"));
            var objectItems = individual.Items.Where(i => i.Value.StartsWith("OBJE"));
            var changeItems = individual.Items.Where(i => i.Value.StartsWith("CHAN"));
            var createItems = individual.Items.Where(i => i.Value.StartsWith("CREA"));
            var familySpouseItems = individual.Items.Where(i => i.Value.StartsWith("FAMS"));
            var familyChildItems = individual.Items.Where(i => i.Value.StartsWith("FAMC"));
            var userDefinedItems = individual.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = individual.Value.GetID("I", 0);
            result.Names = Name.FromDataHierarchy(nameItems, context);
            result.Sex = sexItems.GetValue(4);
            result.Burial = Event.FromDataHierarchy(burialItems, context, Event.EventType.Burial).LastOrDefault();
            result.Death = Event.FromDataHierarchy(deathItems, context, Event.EventType.Death).LastOrDefault();
            result.Birth = Event.FromDataHierarchy(birthItems, context, Event.EventType.Birth).LastOrDefault();
            result.Adoption = Event.FromDataHierarchy(adoptionItems, context, Event.EventType.Adoption).LastOrDefault();
            result.Immigrations = Event.FromDataHierarchy(immigrationItems, context, Event.EventType.Immigration);
            result.Naturalizations = Event.FromDataHierarchy(naturalizationItems, context, Event.EventType.Naturalization);
            result.Emigrations = Event.FromDataHierarchy(emigrationItems, context, Event.EventType.Emigration);
            result.MilitaryServices = Event.FromDataHierarchy(militaryServiceItems, context, Event.EventType.MilitaryService);
            result.Ordinances = Event.FromDataHierarchy(ordinanceItems, context, Event.EventType.Ordinance);
            result.Ordinations = Event.FromDataHierarchy(ordinationItems, context, Event.EventType.Ordination);
            result.Sources = EntitySource.FromDataHierarchy(sourceItems, context);
            result.NoteIDs = noteItems.GetIDs("N");
            result.ObjectIDs = objectItems.GetIDs("M");
            result.Change = DateTime.FromDataHierarchy(changeItems, context, DateTime.DateType.Change).LastOrDefault();
            result.Create = DateTime.FromDataHierarchy(createItems, context, DateTime.DateType.Create).LastOrDefault();
            result.SpousalFamilyIDs = familySpouseItems.GetIDs("F");
            result.ParentFamilyID = familyChildItems.GetID("F");
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
