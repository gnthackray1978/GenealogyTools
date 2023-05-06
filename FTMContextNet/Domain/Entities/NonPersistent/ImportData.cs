using System.Collections.Generic;

namespace FTMContextNet.Domain.Entities.NonPersistent
{
    public class ImportData
    {
        public List<int> CurrentId { get; set; }

        public int NextId { get; set; }
    }
}
