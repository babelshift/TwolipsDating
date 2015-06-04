using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class StoreController : BaseController 
    {
        private StoreService storeService = new StoreService();

        public async Task<ActionResult> Index()
        {
            await SetHeaderCountsAsync();

            return View();
        }

        public async Task<ActionResult> Gifts()
        {
            await SetHeaderCountsAsync();

            StoreViewModel viewModel = new StoreViewModel();
            var gifts = await storeService.GetGiftsAsync();
            viewModel.StoreGifts = Mapper.Map<IReadOnlyCollection<Gift>, IReadOnlyCollection<StoreGiftViewModel>>(gifts);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> BuyGift(int giftId, int giftCount)
        {
            bool success = false;

            try
            {
                if (giftCount > 0 && giftCount <= 100)
                {
                    string userId = await GetCurrentUserIdAsync();

                    int count = await storeService.BuyGiftAsync(userId, giftId, giftCount);

                    success = count > 0;
                }

                return Json(new { success = success, count = giftCount });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = success, error = "Failed to purchase the gift. Contact support if you continue seeing this." });
            }
        }
    }
}