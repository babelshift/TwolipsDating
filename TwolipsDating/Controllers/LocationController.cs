using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;

namespace TwolipsDating.Controllers
{
    public class LocationJsonResult
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
    }

    public class LocationController : Controller
    {
        public async Task<JsonResult> Zip(string id)
        {
            if (Request.IsAuthenticated)
            {
                ProfileService p = new ProfileService();
                var city = await p.GetCityByZipCodeAsync(id);
                if (city != null)
                {
                    return Json(new LocationJsonResult() { CityId = city.Id, CityName = city.Name }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new LocationJsonResult() { CityName = String.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.Forbidden));
            }
        }

        public async Task<JsonResult> City(string id)
        {
            if (Request.IsAuthenticated)
            {
                ProfileService p = new ProfileService();
                var city = await p.GetCityByNameAsync(id);
                if (city != null)
                {
                    return Json(new LocationJsonResult() { CityId = city.Id, CityName = city.Name }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new LocationJsonResult() { CityName = String.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.Forbidden));
            }
        }
    }
}