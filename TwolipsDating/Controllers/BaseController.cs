using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Configuration;
using NLog;
using TwolipsDating.Utilities;

namespace TwolipsDating.Controllers
{
    public class BaseController : Controller
    {
        protected LogHelper Log { get; private set; }

        private readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];
        private ProfileService profileService = new ProfileService();
        private ApplicationUserManager _userManager;

        protected ProfileService ProfileService { get { return profileService; } }

        protected string CDN { get { return cdn; } }

        protected ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public BaseController()
        {
            Log = new LogHelper(GetType().FullName);
        }

        public BaseController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            Log = new LogHelper(GetType().FullName);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.Result = GetActionResult(filterContext);

            Log.Error(filterContext.Exception.Message, filterContext.Exception.StackTrace);

            filterContext.ExceptionHandled = true;
        }

        private ActionResult GetActionResult(ExceptionContext filterContext)
        {
            string actionName = filterContext.RouteData.Values["action"].ToString();
            Type controller = filterContext.Controller.GetType();
            var method = controller.GetMethod(actionName);
            var returnType = method.ReturnType;

            if (returnType.Equals(typeof(JsonResult)))
            {
                return Json(new { success = false, message = "An unhandled exception occurred." });
            }
            else if (returnType.Equals(typeof(ActionResult)) || returnType.IsSubclassOf(typeof(ActionResult)))
            {
                return new ViewResult() { ViewName = "~/Views/Error/Index.cshtml" };
            }
            else
            {
                return null;
            }
        }

        protected async Task SetUnreadCountsInViewBag()
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = await GetCurrentUserIdAsync();
                int unreadMessageCount = await profileService.GetUnreadMessageCountAsync(currentUserId);
                ViewBag.UnreadAnnouncementCount = 0;
                ViewBag.ItemCount = 0;
                ViewBag.GiftCount = 0;
                ViewBag.UnreadMessageCount = unreadMessageCount;
                ViewBag.UnreadNotificationCount = unreadMessageCount;
            }
        }

        protected async Task<string> GetCurrentUserIdAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = String.Empty;

                if (Session["CurrentUserId"] == null || String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
                {
                    var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
                    Session["CurrentUserId"] = currentUser.Id;
                }

                return Session["CurrentUserId"].ToString();
            }

            return String.Empty;
        }


        protected void AddError(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
        }

        protected ActionResult RedirectToIndex(object routeValues = null)
        {
            return RedirectToAction("index", routeValues);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            if (disposing && profileService != null)
            {
                profileService.Dispose();
                profileService = null;
            }

            base.Dispose(disposing);
        }

    }
}