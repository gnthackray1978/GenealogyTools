using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Object : BaseEntity
    {
        public int ID { get; set; }
        public string File { get; set; }
        public string Title { get; set; }
        public IList<int> NoteIDs { get; set; }
        public IList<EntitySource> Sources { get; set; }
        public DateTime Change { get; set; }
        public DateTime Create { get; set; }

        public IEnumerable<Note> Notes
        {
            get
            {
                return Context.Notes.Where(n => NoteIDs.Contains(n.ID));
            }
        }

        public byte[] FileData
        {
            get
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(Context.GEDCOMObjectFolder, File)))
                    return System.IO.File.ReadAllBytes(System.IO.Path.Combine(Context.GEDCOMObjectFolder, File));
                return null;
            }
        }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatObjectString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(File))
                output.AppendLine(FormatFileString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Title))
                output.AppendLine(FormatTitleString(hierarchyRoot + 1));

            if (Change != null)
                output.Append(FormatChangeString(hierarchyRoot + 1));

            if (Create != null)
                output.Append(FormatCreateString(hierarchyRoot + 1));

            if (Sources != null)
                output.Append(FormatSourceString(hierarchyRoot + 1));

            if (NoteIDs != null)
                output.Append(FormatNoteString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatObjectString(int hierarchyRoot)
        {
            return string.Format("{0} @O{1}@ OBJE", hierarchyRoot, ID);
        }

        private string FormatTitleString(int hierarchyRoot)
        {
            return string.Format("{0} TITL {1}", hierarchyRoot, Title);
        }

        private string FormatFileString(int hierarchyRoot)
        {
            return string.Format("{0} FILE {1}", hierarchyRoot, File);
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

        private string FormatSourceString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var source in Sources)
                output.Append(source.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Object> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildObject(i, context)).ToList();
        }

        private static LINQ2GEDCOM.Entities.Object BuildObject(DataHierarchyItem _object, GEDCOMContext context)
        {
            var result = new LINQ2GEDCOM.Entities.Object();
            result.Context = context;

            var titleItems = _object.Items.Where(i => i.Value.StartsWith("TITL"));
            var fileItems = _object.Items.Where(i => i.Value.StartsWith("FILE"));
            var noteItems = _object.Items.Where(i => i.Value.StartsWith("NOTE"));
            var changeItems = _object.Items.Where(i => i.Value.StartsWith("CHAN"));
            var createItems = _object.Items.Where(i => i.Value.StartsWith("CREA"));
            var sourceItems = _object.Items.Where(i => i.Value.StartsWith("SOUR"));
            var userDefinedItems = _object.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = _object.Value.GetID("M", 0);
            result.Title = titleItems.GetValue();
            result.File = fileItems.GetValue();
            result.NoteIDs = noteItems.GetIDs("N");
            result.Change = DateTime.FromDataHierarchy(changeItems, context, DateTime.DateType.Change).LastOrDefault();
            result.Create = DateTime.FromDataHierarchy(createItems, context, DateTime.DateType.Create).LastOrDefault();
            result.Sources = EntitySource.FromDataHierarchy(sourceItems, context);
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
