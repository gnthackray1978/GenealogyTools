using System.Threading.Tasks;
using FTMContextNet.Domain.Commands;
using Microsoft.AspNetCore.Mvc;
using MSG.CommonTypes;

namespace GenDataAPI
{
    public static class CommandResultExtensions
    {
        public static IActionResult ConvertResult(this ControllerBase value, CommandResult cb)
        {
            if (cb.CommandResultType == CommandResultType.Success)
                return value.Ok(cb.Id == "" ? true : cb.Id);

            if (cb.CommandResultType == CommandResultType.RecordExists)
                return value.Conflict(cb.Message == "" ? true : cb.Message);

            if (cb.CommandResultType == CommandResultType.InvalidRequest)
                return value.BadRequest(cb.Message == "" ? true : cb.Message);

            if (cb.CommandResultType == CommandResultType.Unauthorized)
                return value.Forbid(cb.Message);

            return value.Ok();


        }

        
    }
}
