using System;
using System.Collections.Generic;

namespace AzureContext.Models
{
    public partial class Customers
    {
        public int Id { get; set; }
        public string IdentityId { get; set; }
        public string Location { get; set; }
        public string Locale { get; set; }
        public string Gender { get; set; }

        public virtual AspNetUsers Identity { get; set; }
    }
}
