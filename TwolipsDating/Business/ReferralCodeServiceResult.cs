using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class ReferralCodeServiceResult : ServiceResult
    {
        public string Code { get; private set; }

        private ReferralCodeServiceResult(string code)
        {
            Code = code;
        }

        private ReferralCodeServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new ReferralCodeServiceResult Success(string code)
        {
            return new ReferralCodeServiceResult(code);
        }

        public static new ReferralCodeServiceResult Failed(params string[] errors)
        {
            return new ReferralCodeServiceResult(errors);
        }
    }
}