using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class ApiResources
    {
        public ApiResources()
        {
            ApiClaims = new HashSet<ApiClaims>();
            ApiProperties = new HashSet<ApiProperties>();
            ApiScopes = new HashSet<ApiScopes>();
            ApiSecrets = new HashSet<ApiSecrets>();
        }

        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool NonEditable { get; set; }

        public virtual ICollection<ApiClaims> ApiClaims { get; set; }
        public virtual ICollection<ApiProperties> ApiProperties { get; set; }
        public virtual ICollection<ApiScopes> ApiScopes { get; set; }
        public virtual ICollection<ApiSecrets> ApiSecrets { get; set; }
    }
}
