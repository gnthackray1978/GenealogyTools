using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LINQ2GEDCOM.Entities;

namespace LINQ2GEDCOM
{
    internal class GEDCOMDataWriter
    {
        public static void WriteToFile(GEDCOMContext context, string file)
        {
            ValidateFile(file);

            using (StreamWriter sw = new StreamWriter(file))
            {
                WriteToFile<Header>(context.Headers, sw);
                WriteToFile<Individual>(context.Individuals, sw);
                WriteToFile<Family>(context.Families, sw);
                WriteToFile<Source>(context.Sources, sw);
                WriteToFile<Entities.Object>(context.Objects, sw);
                WriteToFile<Note>(context.Notes, sw);
                WriteToFile<Label>(context.Labels, sw);
                sw.WriteLine("0 TRLR");
            }
        }

        private static void ValidateFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentException("File can not be empty.");
        }

        private static void WriteToFile<TEntity>(IList<TEntity> items, StreamWriter writer) where TEntity : BaseEntity, new()
        {
            foreach (var item in items)
                writer.Write(item.ToGEDCOMString(0));
        }
    }
}
