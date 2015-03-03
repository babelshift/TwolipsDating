using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class HomeController : BaseController
    {
        private ProfileService profileService = new ProfileService();

        public async Task<ActionResult> Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                // dashboard
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                DashboardViewModel viewModel = new DashboardViewModel();
                SetUnreadCountsInViewBag(profileService, user);
                return View("dashboard", viewModel);
            }
            else
            {
                // splash
                HomeViewModel viewModel = new HomeViewModel();
                return View(String.Empty, "~/Views/Shared/_LayoutSplash.cshtml", viewModel);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}