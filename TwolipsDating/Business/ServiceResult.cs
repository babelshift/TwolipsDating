﻿using System.Collections.Generic;

namespace TwolipsDating.Business
{
    public class ServiceResult
    {
        public bool Succeeded { get; private set; }
        public IEnumerable<string> Errors { get; private set; }

        protected ServiceResult()
        {
            Succeeded = true;
        }

        public ServiceResult(IEnumerable<string> errors)
        {
            Succeeded = false;
            Errors = errors;
        }

        public static ServiceResult Success
        {
            get { return new ServiceResult(); }
        }

        public static ServiceResult Failed(params string[] errors)
        {
            return new ServiceResult(errors);
        }
    }
}