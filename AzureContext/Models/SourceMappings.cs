using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class SourceMappings
    {
        public int MappingId { get; set; }
        public Guid? SourceId { get; set; }
        public Guid? MarriageRecordId { get; set; }
        public Guid? PersonRecordId { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? MapTypeId { get; set; }
    }
}
