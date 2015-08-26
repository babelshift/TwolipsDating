using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TwolipsDating.Business;
using System.Threading.Tasks;
using System.Web.Routing;

namespace TwolipsDating.Utilities
{
    public class RequireProfile : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                UserService userService = new UserService();
                bool doesUserHaveProfile = userService.DoesUserHaveProfile(currentUserId);
                if (!doesUserHaveProfile)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary 
                        { 
                            { "controller", "Profile" }, 
                            { "action", "Index" } 
                        });
                }
            }
        }
    }
}