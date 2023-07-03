using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTMContextNet.Domain.Entities.NonPersistent
{
    /// <summary>
    /// If the result of our write isn't covered by this.. then it's exceptional :/ and is therefore
    /// an exception.
    /// </summary>
    public class APIResult
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public APIResultType ApiResultType { get; set; }

    }

    public enum APIResultType
    {
        Success = 0,
        RecordExists =1,
        InvalidRequest =2,
        Unauthorized =3
    }
}
