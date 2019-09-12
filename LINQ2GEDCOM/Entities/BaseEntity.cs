using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public abstract class BaseEntity
    {
        public GEDCOMContext Context { get; internal set; }

        public IList<UserDefinedTag> UserDefinedTags { get; set; }

        internal abstract string ToGEDCOMString(int hierarchyRoot);
    }
}
