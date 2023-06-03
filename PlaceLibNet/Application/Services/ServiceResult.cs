using System;

namespace FTMContextNet.Application.Services
{
     
    public class ServiceResult
    {
        private ServiceResult()
        {
        }

        private ServiceResult(string failureReason, string id)
        {
            this.FailureReason = failureReason;
            this.Id = id;
        }

       
        public static ServiceResult Success { get; } = new ServiceResult();

       
        public string FailureReason { get; }

     
        public string Id { get; }

       
        public bool IsSuccess => string.IsNullOrEmpty(this.FailureReason);

      
        public static implicit operator bool(ServiceResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return result.IsSuccess;
        }

     
        public static ServiceResult SuccessWithId(string id)
        {
            return new ServiceResult(string.Empty, id);
        }

     
        public static ServiceResult Fail(string reason)
        {
            return new ServiceResult(reason, string.Empty);
        }

      
        public bool ToBoolean()
        {
            return this.IsSuccess;
        }
    }
}
