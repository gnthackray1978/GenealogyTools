using FTMContextNet.Domain.Entities.NonPersistent;
using Microsoft.AspNetCore.Mvc;

namespace GenDataAPI
{
    public static class APIResultExtensions
    {
        public static ActionResult ConvertResult(this ControllerBase value, APIResult cb)
        {
            if (cb.ApiResultType == APIResultType.Success)
                return value.Ok(cb.Id);

            if (cb.ApiResultType == APIResultType.RecordExists)
                return value.Conflict(cb.Message);

            if (cb.ApiResultType == APIResultType.InvalidRequest)
                return value.BadRequest(cb.Message);

            if (cb.ApiResultType == APIResultType.Unauthorized)
                return value.Forbid(cb.Message);

            return value.Ok();


        }
    }
}
