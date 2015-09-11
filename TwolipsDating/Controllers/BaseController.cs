using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
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

        private ApplicationUserManager userManager;

        private IProfileService profileService;
        private IUserService userService;
        private IDashboardService dashboardService;
        private IMilestoneService milestoneService;
        private INotificationService notificationService;
        private ISearchService searchService;
        private IStoreService storeService;
        private ITriviaService triviaService;
        private IViolationService violationService;

        #endregion Members

        public BaseController(
            ApplicationUserManager userManager,
            IProfileService profileService,
            IUserService userService,
            IDashboardService dashboardService,
            IMilestoneService milestoneService,
            INotificationService notificationService,
            ISearchService searchService,
            IStoreService storeService,
            ITriviaService triviaService,
            IViolationService violationService)
        {
            UserManager = userManager;
            this.profileService = profileService;
            this.userService = userService;
            this.dashboardService = dashboardService;
            this.milestoneService = milestoneService;
            this.notificationService = notificationService;
            this.searchService = searchService;
            this.storeService = storeService;
            this.triviaService = triviaService;
            this.violationService = violationService;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ProfileService.ValidationDictionary = new ModelStateWrapper(ModelState);
            UserService.ValidationDictionary = new ModelStateWrapper(ModelState);
            DashboardService.ValidationDictionary = new ModelStateWrapper(ModelState);
            MilestoneService.ValidationDictionary = new ModelStateWrapper(ModelState);
            NotificationService.ValidationDictionary = new ModelStateWrapper(ModelState);
            SearchService.ValidationDictionary = new ModelStateWrapper(ModelState);
            StoreService.ValidationDictionary = new ModelStateWrapper(ModelState);
            TriviaService.ValidationDictionary = new ModelStateWrapper(ModelState);
            ViolationService.ValidationDictionary = new ModelStateWrapper(ModelState);

            MilestoneService.ProfileService = ProfileService;
            MilestoneService.TriviaService = TriviaService;

            ProfileService.UserService = UserService;
            ProfileService.MilestoneService = MilestoneService;

            UserService.MilestoneService = MilestoneService;
            DashboardService.MilestoneService = MilestoneService;
            MilestoneService.MilestoneService = MilestoneService;
            NotificationService.MilestoneService = MilestoneService;
            SearchService.MilestoneService = MilestoneService;
            StoreService.MilestoneService = MilestoneService;
            TriviaService.MilestoneService = MilestoneService;
            ViolationService.MilestoneService = MilestoneService;
        }

        #region Properties

        protected IProfileService ProfileService
        {
            get { return profileService ?? HttpContext.GetOwinContext().Get<ProfileService>(); }
            private set { profileService = value; }
        }

        protected IUserService UserService
        {
            get { return userService ?? HttpContext.GetOwinContext().Get<UserService>(); }
            private set { userService = value; }
        }

        protected IDashboardService DashboardService
        {
            get { return dashboardService ?? HttpContext.GetOwinContext().Get<DashboardService>(); }
            private set { dashboardService = value; }
        }

        protected IMilestoneService MilestoneService
        {
            get { return milestoneService ?? HttpContext.GetOwinContext().Get<MilestoneService>(); }
            private set { milestoneService = value; }
        }

        protected INotificationService NotificationService
        {
            get { return notificationService ?? HttpContext.GetOwinContext().Get<NotificationService>(); }
            private set { notificationService = value; }
        }

        protected ISearchService SearchService
        {
            get { return searchService ?? HttpContext.GetOwinContext().Get<SearchService>(); }
            private set { searchService = value; }
        }

        protected IStoreService StoreService
        {
            get { return storeService ?? HttpContext.GetOwinContext().Get<StoreService>(); }
            private set { storeService = value; }
        }

        protected ITriviaService TriviaService
        {
            get { return triviaService ?? HttpContext.GetOwinContext().Get<TriviaService>(); }
            private set { triviaService = value; }
        }

        protected IViolationService ViolationService
        {
            get { return violationService ?? HttpContext.GetOwinContext().Get<ViolationService>(); }
            private set { violationService = value; }
        }

        /// <summary>
        /// Allows inherited controllers to properly log any events.
        /// </summary>
        protected LogHelper Log { get; private set; }

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
                ViewBag.MessageNotificationCount = await NotificationService.GetMessageNotificationCountAsync(currentUserId);
                ViewBag.PointsCount = currentUser.Points;

                var announcements = await NotificationService.GetAnnouncementNotificationsAsync();
                ViewBag.Announcements = announcements;
                ViewBag.AnnouncementNotificationCount = announcements.Count;

                var gifts = await ProfileService.GetUnreviewedGiftTransactionsAsync(currentUserId);
                var giftsViewModels = Mapper.Map<IReadOnlyCollection<GiftTransactionLog>, IReadOnlyCollection<GiftTransactionViewModel>>(gifts);
                ViewBag.GiftsReceived = giftsViewModels;
                ViewBag.GiftNotificationCount = giftsViewModels != null ? giftsViewModels.Count : 0;
            }
        }

        /// <summary>
        /// Adds an error to the model state to be displayed back to the view.
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void AddError(string errorMessage, string actionName = null, object parameters = null)
        {
            Log.Error(actionName, errorMessage, parameters);
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
    }
}