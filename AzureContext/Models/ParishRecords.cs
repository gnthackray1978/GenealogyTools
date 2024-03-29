﻿using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class ParishRecords
    {
        public int ParishRecordsId { get; set; }
        public Guid? ParishId { get; set; }
        public int? DataTypeId { get; set; }
        public int? Year { get; set; }
        public string RecordType { get; set; }
        public bool? OriginalRegister { get; set; }
        public int? YearEnd { get; set; }
    }
}
