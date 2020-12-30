using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StringValue { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
