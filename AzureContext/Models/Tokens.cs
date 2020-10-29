using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Tokens
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string Type { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Issued { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Refresh { get; set; }
        public string Locale { get; set; }
        public string ImageUrl { get; set; }
    }
}
