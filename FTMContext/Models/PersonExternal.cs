using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class PersonExternal
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int ProviderId { get; set; }
        public string ExternalId { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual Person Person { get; set; }
    }
}
