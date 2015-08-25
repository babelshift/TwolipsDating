using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwolipsDating.Utilities
{
    public static class UrlHelperExtensions
    {
        public static string ActionWithFullUrl(this UrlHelper urlHelper, HttpRequestBase requestBase, string action, string controller, object parameters)
        {
            return requestBase.Url.Scheme + "://" + requestBase.Url.Authority + urlHelper.Action(action, controller, parameters);
        }
    }
}