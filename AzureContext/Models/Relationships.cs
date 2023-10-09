using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureContext.Models
{
    public partial class Relationships
    {
        public int Id { get; set; }

        public int GroomId { get; set; }

        public int BrideId { get; set; }
        
        public string Notes { get; set; }
        public string DateStr { get; set; }
        public int Year { get; set; }

        public string Location { get; set; }

        public string Origin { get; set; }

        public int UserId { get; set; }

        public int ImportId { get; set; }
    }
}
