using System.Threading.Tasks;
using System.Web.Mvc;

namespace TwolipsDating.Controllers
{
    [AllowAnonymous]
    public class AboutController : BaseController
    {
        /// <summary>
        /// Shows the standard about page.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            await SetNotificationsAsync();
            return View();
        }

        /// <summary>
        /// Shows the terms of service.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Terms()
        {
            await SetNotificationsAsync();
            return View();
        }

        /// <summary>
        /// Shows the privacy policy.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Privacy()
        {
            await SetNotificationsAsync();
            return View();
        }
    }
}