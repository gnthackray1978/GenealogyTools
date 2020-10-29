using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class ParishTranscriptionDetails
    {
        public int ParishTranscriptionId { get; set; }
        public Guid? ParishId { get; set; }
        public string ParishDataString { get; set; }
    }
}
