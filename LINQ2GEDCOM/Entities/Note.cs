using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Note : BaseEntity
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime Change { get; set; }
        public DateTime Create { get; set; }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatNoteString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Text))
                output.Append(FormatTextString(hierarchyRoot + 1));

            if (Change != null)
                output.Append(FormatChangeString(hierarchyRoot + 1));

            if (Create != null)
                output.Append(FormatCreateString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatNoteString(int hierarchyRoot)
        {
            return string.Format("{0} @N{1}@ NOTE", hierarchyRoot, ID.ToString());
        }

        private string FormatTextString(int hierarchyRoot)
        {
            var result = new StringBuilder();

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
        #endregion

        #region Input from GEDCOM
        internal static IList<Note> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context)
        {
            return items.Select(i => BuildNote(i, context)).ToList();
        }

        private static Note BuildNote(DataHierarchyItem note, GEDCOMContext context)
        {
            var result = new Note();
            result.Context = context;

            var textItems = note.Items.Where(i => (i.Value.StartsWith("CONT") || i.Value.StartsWith("CONC")));
            var changeItems = note.Items.Where(i => i.Value.StartsWith("CHAN"));
            var createItems = note.Items.Where(i => i.Value.StartsWith("CREA"));
            var userDefinedItems = note.Items.Where(i => i.Value.StartsWith("_"));

            result.ID = note.Value.GetID("N", 0);

            foreach (var textItem in textItems)
                if (textItem.Value.StartsWith("CONT"))
                    result.Text += Environment.NewLine + textItem.Value.GetSubstring(5);
                else if (textItem.Value.StartsWith("CONC"))
                    result.Text += textItem.Value.GetSubstring(5);
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
