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
using Microsoft.AspNet.Identity;
using TwolipsDating.Utilities;

namespace TwolipsDating.Controllers
{
    public class StoreController : BaseController
    {
        private StoreService storeService = new StoreService();
        private UserService userService = new UserService();

        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            return View();
        }

        public async Task<ActionResult> Gifts()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            StoreViewModel viewModel = new StoreViewModel();
            var gifts = await storeService.GetGiftsAsync();
            viewModel.StoreGifts = Mapper.Map<IReadOnlyCollection<Gift>, IReadOnlyCollection<StoreGiftViewModel>>(gifts);

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        public async Task<ActionResult> Titles()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            StoreViewModel viewModel = new StoreViewModel();
            var titles = await storeService.GetTitlesAsync();
            viewModel.StoreTitles = Mapper.Map<IReadOnlyCollection<Title>, IReadOnlyCollection<StoreTitleViewModel>>(titles);

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            var titlesOwnedByUser = await userService.GetTitlesOwnedByUserAsync(currentUserId);

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

            string currentUserId = User.Identity.GetUserId();

            try
            {
                if (giftCount > 0 && giftCount <= 100)
                {
                    int count = await storeService.BuyGiftAsync(currentUserId, giftId, giftCount);

                    success = count > 0;
                }

                return Json(new { success = success, count = giftCount });
            }
            catch (DbUpdateException e)
            {
                Log.Error("BuyGift", e,
                    parameters: new { giftId = giftId, giftCount = giftCount, currentUserId = currentUserId }
                );

                return Json(new { success = success, error = ErrorMessages.GiftPurchaseFailed });
            }
        }

        [HttpPost]
        public async Task<JsonResult> BuyTitle(int titleId)
        {
            bool success = false;

            string currentUserId = User.Identity.GetUserId();

            try
            {
                int count = await storeService.BuyTitleAsync(currentUserId, titleId);

                success = count > 0;

                return Json(new { success = success });
            }
            catch (DbUpdateException e)
            {
                Log.Error("BuyGift", e,
                    parameters: new { titleId = titleId, currentUserId = currentUserId }
                );

                return Json(new { success = success, error = ErrorMessages.TitlePurchaseFailed });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && storeService != null)
            {
                storeService.Dispose();
            }

            if (disposing && userService != null)
            {
                userService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}