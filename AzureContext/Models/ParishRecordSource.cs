using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class ParishRecordSource
    {
        public int RecordTypeId { get; set; }
        public string RecordTypeName { get; set; }
        public string RecordTypeDescription { get; set; }
    }
}
