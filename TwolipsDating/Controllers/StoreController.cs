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

        public async Task<ActionResult> Titles()
        {
            await SetHeaderCountsAsync();

            StoreViewModel viewModel = new StoreViewModel();
            var titles = await storeService.GetTitlesAsync();
            viewModel.StoreTitles = Mapper.Map<IReadOnlyCollection<Title>, IReadOnlyCollection<StoreTitleViewModel>>(titles);

            var currentUserId = await GetCurrentUserIdAsync();
            var titlesOwnedByUser = await ProfileService.GetTitlesOwnedByUserAsync(currentUserId);

            foreach (var title in viewModel.StoreTitles)
            {
                if (titlesOwnedByUser.Any(q => q.Key == title.TitleId))
                {
                    title.IsAlreadyOwnedByUser = true;
                }
            }

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
                    string currentUserId = await GetCurrentUserIdAsync();

                    int count = await storeService.BuyGiftAsync(currentUserId, giftId, giftCount);

                    success = count > 0;
                }

                return Json(new { success = success, count = giftCount });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = success, error = "Failed to purchase the gift. Contact support if you continue seeing this." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> BuyTitle(int titleId)
        {
            bool success = false;

            try
            {
                string currentUserId = await GetCurrentUserIdAsync();

                int count = await storeService.BuyTitleAsync(currentUserId, titleId);

                success = count > 0;

                return Json(new { success = success });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = success, error = "Failed to purchase the title. Contact support if you continue seeing this." });
            }
        }
    }
}