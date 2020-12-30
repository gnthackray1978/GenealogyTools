using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Task
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public int Category { get; set; }
        public int TaskPriority { get; set; }
        public int CreateDate { get; set; }
        public int DueDate { get; set; }
        public bool Complete { get; set; }
        public bool SystemTask { get; set; }
        public string TaskText { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }
    }
}
