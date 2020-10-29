using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class IdentityResources
    {
        public IdentityResources()
        {
            IdentityClaims = new HashSet<IdentityClaims>();
            IdentityProperties = new HashSet<IdentityProperties>();
        }

        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool NonEditable { get; set; }

        public virtual ICollection<IdentityClaims> IdentityClaims { get; set; }
        public virtual ICollection<IdentityProperties> IdentityProperties { get; set; }
    }
}
