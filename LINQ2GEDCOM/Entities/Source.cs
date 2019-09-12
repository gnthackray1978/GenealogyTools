using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Source : BaseEntity
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public IList<int> NoteIDs { get; set; }
        public IList<int> ObjectIDs { get; set; }
        public DateTime Change { get; set; }
        public DateTime Create { get; set; }

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

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatSourceString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Title))
                output.AppendLine(FormatTitleString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Author))
                output.AppendLine(FormatAuthorString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Date))
                output.AppendLine(FormatDateString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Text))
                output.Append(FormatTextString(hierarchyRoot + 1));

            if (NoteIDs != null)
                output.Append(FormatNoteString(hierarchyRoot + 1));

            if (ObjectIDs != null)
                output.Append(FormatObjectsString(hierarchyRoot + 1));

            if (Change != null)
                output.Append(FormatChangeString(hierarchyRoot + 1));

            if (Create != null)
                output.Append(FormatCreateString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatSourceString(int hierarchyRoot)
        {
            return string.Format("{0} @S{1}@ SOUR", hierarchyRoot, ID.ToString());
        }

        private string FormatTitleString(int hierarchyRoot)
        {
            return string.Format("{0} TITL {1}", hierarchyRoot, Title);
        }

        private string FormatAuthorString(int hierarchyRoot)
        {
            return string.Format("{0} AUTH {1}", hierarchyRoot, Author);
        }

        private string FormatDateString(int hierarchyRoot)
        {
            return string.Format("{0} DATE {1}", hierarchyRoot, Date);
        }

        private string FormatTextString(int hierarchyRoot)
        {
            var result = new StringBuilder();
            result.AppendLine(string.Format("{0} TEXT", hierarchyRoot));
            hierarchyRoot++;

            var linesOfText = Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var lineOfText in linesOfText)
            {
                if (lineOfText.Length <= 255)
                    if (string.IsNullOrWhiteSpace(lineOfText))
                        result.AppendLine(string.Format("{0} CONT", hierarchyRoot));
                    else
                        result.AppendLine(string.Format("{0} CONT {1}", hierarchyRoot, lineOfText));
                else
                {
                    var lines = lineOfText.SplitIntoChunks(255);
                    if (lines.Count() > 0)
                    {
                        result.AppendLine(string.Format("{0} CONT {1}", hierarchyRoot, lines.First()));
                        foreach (var line in lines.Skip(1))
                            result.AppendLine(string.Format("{0} CONC {1}", hierarchyRoot, line));
                    }
                }
            }

            return result.ToString();
        }

        private string FormatChangeString(int hierarchyRoot)
        {
            return Change.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatCreateString(int hierarchyRoot)
        {
            return Create.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatNoteString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var noteID in NoteIDs)
                output.AppendLine(string.Format("{0} NOTE @N{1}@", hierarchyRoot, noteID));
            return output.ToString();
        }

        private string FormatObjectsString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var objectID in ObjectIDs)
                output.AppendLine(string.Format("{0} OBJE @O{1}@", hierarchyRoot, objectID));
            return output.ToString();
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Source> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildSource(i, context)).ToList();
        }

        private static Source BuildSource(DataHierarchyItem source, GEDCOMContext context)
        {
            var result = new Source();
            result.Context = context;

            var titleItems = source.Items.Where(i => i.Value.StartsWith("TITL"));
            var authorItems = source.Items.Where(i => i.Value.StartsWith("AUTH"));
            var dateItems = source.Items.Where(i => i.Value.StartsWith("DATE"));
            var noteItems = source.Items.Where(i => i.Value.StartsWith("NOTE"));
            var textItems = source.Items.Where(i => i.Value.StartsWith("TEXT"));
            var objectItems = source.Items.Where(i => i.Value.StartsWith("OBJE"));
            var changeItems = source.Items.Where(i => i.Value.StartsWith("CHAN"));
            var createItems = source.Items.Where(i => i.Value.StartsWith("CREA"));
            var userDefinedItems = source.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = source.Value.GetID("S", 0);
            result.Title = titleItems.GetValue();
            result.Author = authorItems.GetValue();
            result.Date = dateItems.GetValue();
            result.NoteIDs = noteItems.GetIDs("N");
            result.ObjectIDs = objectItems.GetIDs("O");
            foreach (var textItem in textItems)
            {
                var contItems = textItem.Items.Where(i => (i.Value.StartsWith("CONT") || i.Value.StartsWith("CONC")));
                foreach (var contItem in contItems)
                    if (contItem.Value.StartsWith("CONT"))
                        result.Text += Environment.NewLine + contItem.Value.GetSubstring(5);
                    else if (contItem.Value.StartsWith("CONC"))
                        result.Text += contItem.Value.GetSubstring(5);
            }
            if (!string.IsNullOrWhiteSpace(result.Text))
                while (result.Text.StartsWith(Environment.NewLine))
                    result.Text = result.Text.Substring(Environment.NewLine.Length);
            result.Change = DateTime.FromDataHierarchy(changeItems, context, DateTime.DateType.Change).LastOrDefault();
            result.Create = DateTime.FromDataHierarchy(createItems, context, DateTime.DateType.Create).LastOrDefault();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
