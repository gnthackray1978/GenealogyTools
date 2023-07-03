using System.Collections.Generic;

namespace FTMContextNet.Domain.Entities.NonPersistent
{
    public class ImportData
    {
        /// <summary>
        /// Made this a list of Ids as I was concerned the multiple versions of
        /// the same tree might get into the DB.
        /// </summary>
        public List<int> CurrentId { get; set; }

        public int NextId { get; set; }
    }
}
