
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
			await SetHeaderCounts();
			return View();
		}

		public async Task<ActionResult> Privacy()
		{
			await SetHeaderCounts();
			return View();
		}

		public async Task<ActionResult> Index()
		{
			await SetHeaderCounts();
            return View();
        }
    }
}