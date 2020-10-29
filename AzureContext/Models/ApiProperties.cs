using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class ApiProperties
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int ApiResourceId { get; set; }

        public virtual ApiResources ApiResource { get; set; }
    }
}
