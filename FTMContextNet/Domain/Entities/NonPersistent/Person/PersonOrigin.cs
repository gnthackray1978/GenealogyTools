using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTMContextNet.Domain.Entities.NonPersistent.Person
{
    public class PersonOrigin
    {
        public string Origin { get; set; }

        public bool DirectAncestor { get; set; }

    }
}
