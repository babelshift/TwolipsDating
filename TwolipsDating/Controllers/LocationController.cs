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
    public class LocationController : BaseController
    {
        //public async Task<JsonResult> Zip(string id)
        //{
        //    var city = await ProfileService.GetCityByZipCodeAsync(id);
        //    if (city != null)
        //    {
        //        return Json(new LocationJsonResult() { CityId = city.Id, CityName = city.Name }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new LocationJsonResult() { CityName = String.Empty }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public async Task<JsonResult> City(string id)
        //{
        //    var city = await ProfileService.GetCityByNameAsync(id);
        //    if (city != null)
        //    {
        //        return Json(new LocationJsonResult() { CityId = city.Id, CityName = city.Name }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new LocationJsonResult() { CityName = String.Empty }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}