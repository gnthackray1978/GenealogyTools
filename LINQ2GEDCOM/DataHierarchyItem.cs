using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM
{
    internal class DataHierarchyItem
    {
        internal string Value { get; set; }
        internal IList<DataHierarchyItem> Items { get; set; }

        internal DataHierarchyItem()
        {
            Value = string.Empty;
            Items = new List<DataHierarchyItem>();
        }
    }
}
