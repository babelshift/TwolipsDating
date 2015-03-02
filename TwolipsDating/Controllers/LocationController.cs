using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public JsonResult Zip(string id)
        {
            if (Request.IsAuthenticated)
            {
                ProfileService p = new ProfileService();
                var city = p.GetCityByZipCode(id);
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

        public JsonResult City(string id)
        {
            if (Request.IsAuthenticated)
            {
                ProfileService p = new ProfileService();
                var city = p.GetCityByName(id);
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