using FTMContextNet.Domain.Commands;
using Microsoft.AspNetCore.Mvc;

namespace GenDataAPI
{
    public static class CommandResultExtensions
    {
        public static ActionResult ConvertResult(this ControllerBase value, CommandResult cb)
        {
            if (cb.CommandResultType == CommandResultType.Success)
                return value.Ok(cb.Id);

            if (cb.CommandResultType == CommandResultType.RecordExists)
                return value.Conflict(cb.Message);

            if (cb.CommandResultType == CommandResultType.InvalidRequest)
                return value.BadRequest(cb.Message);

            if (cb.CommandResultType == CommandResultType.Unauthorized)
                return value.Forbid(cb.Message);

            return value.Ok();


        }
    }
}
