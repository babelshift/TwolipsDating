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

namespace TwolipsDating.Controllers
{
    public class BaseController : Controller
    {
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

		public BaseController() { }

		public BaseController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

			if(disposing && profileService != null)
			{
				profileService.Dispose();
				profileService = null;
			}

            base.Dispose(disposing);
        }
    }
}