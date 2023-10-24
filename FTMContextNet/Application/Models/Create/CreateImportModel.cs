using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTMContextNet.Application.Models.Create
{
    public class CreateImportModel
    {
        public string FileName { get; set; }

        public string FileSize { get; set; }

        public bool Selected { get; set; }
        
    }
}
