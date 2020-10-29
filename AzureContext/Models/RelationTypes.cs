using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class RelationTypes
    {
        public int RelationTypeId { get; set; }
        public string RelationName { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
