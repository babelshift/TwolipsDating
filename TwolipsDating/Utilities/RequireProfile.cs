using Microsoft.AspNet.Identity;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TwolipsDating.Business;
using Microsoft.AspNet.Identity.Owin;

namespace TwolipsDating.Utilities
{
    /// <summary>
    /// Some controllers actions require the user to have a profile in order to execute. If the user hasn't created a profile yet, redirect
    /// them to the profile creation page.
    /// </summary>
    public class RequireProfileIfAuthenticated : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                UserService userService = HttpContext.Current.GetOwinContext().Get<UserService>();
                bool doesUserHaveProfile = userService.DoesUserHaveProfile(currentUserId);
                if (!doesUserHaveProfile)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var behavior = filterContext.HttpContext.Request.HttpMethod.ToUpper() == "GET" ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet;

                        filterContext.Result = new JsonResult
                        {
                            Data = new { success = false, message = ErrorMessages.ProfileIsRequired },
                            JsonRequestBehavior = behavior
                        };
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                                { "controller", "profile" },
                                { "action", "index" }
                            });
                    }
                }
            }
        }
    }
}