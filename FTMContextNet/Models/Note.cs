using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Note
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public string Category { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool Private { get; set; }
        public string NoteText { get; set; }
        public string Uid { get; set; }
    }
}
