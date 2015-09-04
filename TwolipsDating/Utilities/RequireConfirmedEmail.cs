using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace TwolipsDating.Utilities
{
    public class RequireConfirmedEmail : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            if (userManager != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                    bool isCurrentUserEmailConfirmed = userManager.IsEmailConfirmed(currentUserId);
                    if (!isCurrentUserEmailConfirmed)
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new { success = false, message = ErrorMessages.UnhandledException }
                            };
                        }
                        else
                        {
                            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                        }
                    }
                }
            }
        }
    }
}