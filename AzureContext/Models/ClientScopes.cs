using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class ClientScopes
    {
        public int Id { get; set; }
        public string Scope { get; set; }
        public int ClientId { get; set; }

        public virtual Clients Client { get; set; }
    }
}
