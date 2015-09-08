using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class ToggleServiceResult : ServiceResult
    {
        public bool ToggleStatus { get; private set; }

        private ToggleServiceResult(bool toggleStatus)
        {
            ToggleStatus = toggleStatus;
        }

        private ToggleServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new ToggleServiceResult Success(bool toggleStatus)
        {
            return new ToggleServiceResult(toggleStatus);
        }

        public static new ToggleServiceResult Failed(params string[] errors)
        {
            return new ToggleServiceResult(errors);
        }
    }
}