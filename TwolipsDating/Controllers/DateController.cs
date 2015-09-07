using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Utilities;

namespace TwolipsDating.Controllers
{
    public class DateController : BaseController
    {
        public JsonResult DaysOfMonth(int month)
        {
            var days = CalendarHelper.GetDaysOfMonth(month).ToDictionary(m => m.ToString(), m => m.ToString());
            return Json(new { success = true, days = days });
        }
    }
}