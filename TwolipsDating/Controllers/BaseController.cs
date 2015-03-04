using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace TwolipsDating.Controllers
{
    public class BaseController : Controller
    {
        private ProfileService profileService = new ProfileService();
        private ApplicationUserManager _userManager;

        protected ProfileService ProfileService { get { return profileService; } }

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

        protected async Task<ApplicationUser> GetCurrentUserAsync()
        {
                ApplicationUser currentUser = Session["CurrentUser"] as ApplicationUser;
                if(currentUser == null)
                {
                    currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
                    Session["CurrentUser"] = currentUser;
                }
                return currentUser;
        }

        protected void SetUnreadCountsInViewBag(ProfileService p, ApplicationUser user)
        {
            int unreadMessageCount = p.GetUnreadMessageCount(user.Id);
            ViewBag.UnreadAnnouncementCount = 0;
            ViewBag.ItemCount = 0;
            ViewBag.GiftCount = 0;
            ViewBag.UnreadMessageCount = unreadMessageCount;
            ViewBag.UnreadNotificationCount = unreadMessageCount;
        }
    }
}