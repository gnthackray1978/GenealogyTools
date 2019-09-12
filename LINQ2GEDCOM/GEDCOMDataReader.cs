using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LINQ2GEDCOM.Entities;

namespace LINQ2GEDCOM
{
    internal class GEDCOMDataReader
    {
        private GEDCOMDataReader() { }

        private static IList<DataHierarchyItem> GEDCOMDataItems { get; set; }

        internal static void LoadDataIntoContext(GEDCOMContext context)
        {
            InitializeDataHierarchy(context.GEDCOMFile);
            context.Headers = Header.FromDataHierarchy(GEDCOMDataItems.Where(i => i.Value.EndsWith("HEAD")), context);
            context.Individuals = Individual.FromDataHierarchy(GEDCOMDataItems.Where(i => i.Value.EndsWith(" INDI")), context);
            context.Families = Family.FromDataHierarchy(GEDCOMDataItems.Where(i => i.Value.EndsWith(" FAM")), context);
            context.Sources = Source.FromDataHierarchy(GEDCOMDataItems.Where(i => i.Value.EndsWith(" SOUR")), context);
            context.Objects = LINQ2GEDCOM.Entities.Object.FromDataHierarchy(GEDCOMDataItems.Where(i => i.Value.EndsWith(" OBJE")), context);
            context.Notes = Note.FromDataHierarchy(GEDCOMDataItems.Where(i => i.Value.EndsWith(" NOTE")), context);
            context.Labels = Label.FromDataHierarchy(GEDCOMDataItems.Where(i => i.Value.EndsWith(" LABL")), context);
            DestroyDataHierarchy();
        }

        private static void InitializeDataHierarchy(string file)
        {
            ValidateFile(file);

            GEDCOMDataItems = new List<DataHierarchyItem>();

            foreach (var currentLine in File.ReadLines(file))
            {
                var hierarchyIndentationNumber = GetHierarchyNumber(currentLine);

                var currentHierarchyItem = new DataHierarchyItem();
                currentHierarchyItem.Value = currentLine.Substring(2);

                AddItemToHierarchy(hierarchyIndentationNumber, currentHierarchyItem);
            }
        }

        private static void ValidateFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentException("File can not be empty.");
            if (!File.Exists(file))
                throw new ArgumentException("File not found.");
        }

        private static int GetHierarchyNumber(string line)
        {
            var hierarchyIndentationNumber = -1;
            int.TryParse(line.Substring(0, 1), out hierarchyIndentationNumber);
            if (hierarchyIndentationNumber == -1)
                throw new Exception(string.Format("Unable to parse GEDCOM file line: {0}", line));

            return hierarchyIndentationNumber;
        }

        private static void AddItemToHierarchy(int hierarchyIndentationNumber, DataHierarchyItem item)
        {
            switch (hierarchyIndentationNumber)
            {
                case 0:
                    GEDCOMDataItems.Add(item);
                    break;
                case 1:
                    GEDCOMDataItems.Last().Items.Add(item);
                    break;
                case 2:
                    GEDCOMDataItems.Last().Items.Last().Items.Add(item);
                    break;
                case 3:
                    GEDCOMDataItems.Last().Items.Last().Items.Last().Items.Add(item);
                    break;
                case 4:
                    GEDCOMDataItems.Last().Items.Last().Items.Last().Items.Last().Items.Add(item);
                    break;
            }
        }

        // TODO: Test this
        private static IList<DataHierarchyItem> GetHierarchyPosition(int hierarchyIndentationNumber, IList<DataHierarchyItem> hierarchy)
        {
            if (hierarchyIndentationNumber == 0)
                return hierarchy;
            return GetHierarchyPosition(hierarchyIndentationNumber - 1, hierarchy.Last().Items);
        }

        private static void DestroyDataHierarchy()
        {
            GEDCOMDataItems = null;
        }
    }
}
