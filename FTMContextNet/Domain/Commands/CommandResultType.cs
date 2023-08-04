using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTMContextNet.Domain.Commands
{
    public enum CommandResultType
    {
        Success = 0,
        RecordExists = 1,
        InvalidRequest = 2,
        Unauthorized = 3
    }
}
