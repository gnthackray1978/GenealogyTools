using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class SourceMappingTypes
    {
        public int SourceMapTypeId { get; set; }
        public string SourceMapTypeDescription { get; set; }
        public Guid? SourceId { get; set; }
    }
}
