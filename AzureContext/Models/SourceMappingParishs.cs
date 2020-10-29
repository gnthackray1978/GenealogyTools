using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class SourceMappingParishs
    {
        public int SourceMappingParishsRowId { get; set; }
        public Guid? SourceMappingParishId { get; set; }
        public Guid? SourceMappingSourceId { get; set; }
        public DateTime? SourceMappingDateAdded { get; set; }
        public int? SourceMappingUser { get; set; }
    }
}
