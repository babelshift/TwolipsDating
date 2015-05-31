
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TwolipsDating.Controllers
{
    [AllowAnonymous]
    public class AboutController : BaseController
	{
		public async Task<ActionResult> Terms()
		{
			await SetHeaderCountsAsync();
			return View();
		}

		public async Task<ActionResult> Privacy()
		{
			await SetHeaderCountsAsync();
			return View();
		}

		public async Task<ActionResult> Index()
		{
			await SetHeaderCountsAsync();
            return View();
        }
    }
}