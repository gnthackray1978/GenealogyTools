using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Deleted
    {
        public int ItemId { get; set; }
        public int TableId { get; set; }
        public DateTime DeleteDate { get; set; }
    }
}
