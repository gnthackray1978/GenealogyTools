using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Repository
    {
        public Repository()
        {
            MasterSource = new HashSet<MasterSource>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual ICollection<MasterSource> MasterSource { get; set; }
    }
}
