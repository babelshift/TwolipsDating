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
        public async Task<ActionResult> Index()
        {
            var tags = await ProfileService.GetAllTagsInUseAndCountsAsync();

            return View(tags);
        }
    }
}