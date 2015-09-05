using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class ServiceResult
    {
        public bool Succeeded { get; private set; }
        public IEnumerable<string> Errors { get; private set; }

        public ServiceResult(bool success)
        {
            Succeeded = success;
        }

        public ServiceResult(bool success, IEnumerable<string> errors)
        {
            Succeeded = success;
            Errors = errors;
        }

        public static ServiceResult Success
        {
            get { return new ServiceResult(true); }
        }

        public static ServiceResult Failed(params string[] errors)
        {
            return new ServiceResult(false, errors);
        }
    }
}