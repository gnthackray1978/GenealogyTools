using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class HistoryList
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ActionDescription { get; set; }
        public DateTime HistoryDate { get; set; }
        public int ObjectType { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }
    }
}
