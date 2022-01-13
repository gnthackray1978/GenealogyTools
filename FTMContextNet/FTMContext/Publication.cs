using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class Publication
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Attributes { get; set; }
        public int Context { get; set; }
        public int Size { get; set; }
        public string Xml { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Uid { get; set; }
    }
}
