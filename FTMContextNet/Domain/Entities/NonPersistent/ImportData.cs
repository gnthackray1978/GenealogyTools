using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTMContextNet.Domain.Entities.NonPersistent
{
    public class ImportData
    {
        public List<int> CurrentId { get; set; }

        public int NextId { get; set; }
    }
}
