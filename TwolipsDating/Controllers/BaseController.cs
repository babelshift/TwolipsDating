using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using Microsoft.AspNet.Identity.Owin;

namespace TwolipsDating.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
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