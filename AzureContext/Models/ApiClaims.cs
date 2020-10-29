using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class ApiClaims
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int ApiResourceId { get; set; }

        public virtual ApiResources ApiResource { get; set; }
    }
}
