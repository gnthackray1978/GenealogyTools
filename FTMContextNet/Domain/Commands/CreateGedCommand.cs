using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace FTMContextNet.Domain.Commands
{
    public class CreateGedCommand
    {
        public string FileName { get; set; }
        public double FileSize { get; set; }
        public bool Selected { get; set; }

        public CreateGedCommand(string fileName, double fileSize, bool selected)
        {
            this.FileName = fileName;
            this.FileSize = fileSize;
            this.Selected = selected;

        }
    }
}
