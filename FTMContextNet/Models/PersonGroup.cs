using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class PersonGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public int? Color { get; set; }
        public int? RootPersonId { get; set; }
        public int? SubgroupFfid { get; set; }
        public int? SubgroupFmid { get; set; }
        public int? SubgroupMfid { get; set; }
        public int? SubgroupMmid { get; set; }
    }
}
