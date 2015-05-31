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
using System.Net;

namespace TwolipsDating.Controllers
{
    public class BaseController : Controller
    {
        private readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];
        private ProfileService profileService = new ProfileService();
        private ApplicationUserManager userManager;

        protected LogHelper Log { get; private set; }

        protected ProfileService ProfileService { get { return profileService; } }

        protected string CDN { get { return cdn; } }

        protected ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
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
#if !DEBUG
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            filterContext.Result = GetActionResult(filterContext);

            Log.Error(filterContext.Exception.Message, filterContext.Exception.StackTrace);

            filterContext.ExceptionHandled = true;
#endif
        }

        private ActionResult GetActionResult(ExceptionContext filterContext)
        {
            if(filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonResult jsonResult = Json(new { success = false, message = ErrorMessages.UnhandledException });
                jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jsonResult;
            }
            else
            {
                return new ViewResult() { ViewName = "~/Views/Shared/Error.cshtml" };
            }
        }

        protected async Task SetHeaderCountsAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = await GetCurrentUserIdAsync();
                var currentUser = await UserManager.FindByIdAsync(currentUserId);
                int unreadMessageCount = await profileService.GetUnreadMessageCountAsync(currentUserId);
                ViewBag.UnreadAnnouncementCount = 0;
                ViewBag.ItemCount = 0;
                ViewBag.GiftCount = 0;
                ViewBag.UnreadMessageCount = unreadMessageCount;
                ViewBag.UnreadNotificationCount = unreadMessageCount;
                ViewBag.PointsCount = currentUser.Points;
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
            ModelState.AddModelError(Guid.NewGuid().ToString(), errorMessage);
        }

        protected ActionResult RedirectToIndex(object routeValues = null)
        {
            return RedirectToAction("index", routeValues);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
                userManager = null;
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