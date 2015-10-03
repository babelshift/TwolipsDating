using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Utilities
{
    public static class HttpContextExtensions
    {
        public static bool AreAdsEnabled(this HttpContext context)
        {
            return false;
        }
    }
}