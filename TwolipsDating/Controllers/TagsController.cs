using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class TagsController : BaseController
    {
        // GET: Tags
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            await SetNotificationsAsync();

            var tags = await ProfileService.GetAllTagsInUseAndCountsAsync();

            return View(tags);
        }
    }
}