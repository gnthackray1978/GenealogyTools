using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class TreeGroups
    {
        public int Id { get; set; }

        public string GroupName { get; set; }

        public int ImportId { get; set; }

    }
}
