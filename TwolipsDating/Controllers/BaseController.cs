using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class BaseController : Controller
    {
        #region Members

        private ProfileService profileService = new ProfileService();
        private NotificationService notificationService = new NotificationService();
        private ApplicationUserManager userManager;

        #endregion Members

        #region Properties

        /// <summary>
        /// Allows inherited controllers to properly log any events.
        /// </summary>
        protected LogHelper Log { get; private set; }

        /// <summary>
        /// Allows inherited controllers access to any services related to a user profile.
        /// </summary>
        protected ProfileService ProfileService { get { return profileService; } }

        /// <summary>
        /// Allows inherited controllers access to managing the user entities.
        /// </summary>
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

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor establishes a log to use
        /// </summary>
        public BaseController()
        {
            Log = new LogHelper(GetType().FullName);
        }

        /// <summary>
        /// Constructor with user manager overload allows specific user manager and log setup
        /// </summary>
        /// <param name="userManager"></param>
        public BaseController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            Log = new LogHelper(GetType().FullName);
        }

        #endregion Constructors

        /// <summary>
        /// Global OnException filter which is used by MVC when an exception occurs in a controller. Only works in production mode.
        /// Does nothing if the exception is already handled. If the exception is unhandled, the status code is set to 500 and returned.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
#if !DEBUG
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            filterContext.Result = GetActionResultOnException(filterContext);

            Log.Error(filterContext.Exception.Message, filterContext.Exception.StackTrace);

            filterContext.ExceptionHandled = true;
#endif
        }

        /// <summary>
        /// Gets the appropriate result to send back on an exception. If the request is an AJAX request, JSON is returned. Otherwise, the standard Error view is displayed.
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private ActionResult GetActionResultOnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
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

        /// <summary>
        /// Sets up the ViewBag to contain notification counts and collections such as messages, gifts received, and announcements.
        /// </summary>
        /// <returns></returns>
        protected async Task SetNotificationsAsync()
        {
            // notifications are only useful for logged in users
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.Identity.GetUserId();
                var currentUser = await UserManager.FindByIdAsync(currentUserId);
                ViewBag.MessageNotificationCount = await notificationService.GetMessageNotificationCountAsync(currentUserId);
                ViewBag.PointsCount = currentUser.Points;

                ViewBag.Announcements = await notificationService.GetAnnouncementNotificationsAsync();
                ViewBag.AnnouncementNotificationCount = ViewBag.Announcements.Count;

                var gifts = await profileService.GetUnreviewedGiftTransactionsAsync(currentUserId);
                ViewBag.GiftsReceived = Mapper.Map<IReadOnlyCollection<GiftTransactionLog>, IReadOnlyCollection<GiftTransactionViewModel>>(gifts);
                ViewBag.GiftNotificationCount = ViewBag.GiftsReceived != null ? ViewBag.GiftsReceived.Count : 0;
            }
        }

        /// <summary>
        /// Adds an error to the model state to be displayed back to the view.
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void AddError(string errorMessage)
        {
            ModelState.AddModelError(Guid.NewGuid().ToString(), errorMessage);
        }

        /// <summary>
        /// Redirects the controller to the /home/index action.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        protected ActionResult RedirectToIndex(object routeValues = null)
        {
            return RedirectToAction("index", routeValues);
        }

        /// <summary>
        /// Redirects the controller to the /profile/index action.
        /// </summary>
        /// <returns></returns>
        protected ActionResult RedirectToProfileIndex()
        {
            return RedirectToAction("index", "profile");
        }

        /// <summary>
        /// Disposes of the services and their associated DbContexts.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (userManager != null)
                {
                    userManager.Dispose();
                    userManager = null;
                }

                if (profileService != null)
                {
                    profileService.Dispose();
                    profileService = null;
                }

                if (notificationService != null)
                {
                    notificationService.Dispose();
                    notificationService = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}