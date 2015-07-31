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

            StoreViewModel viewModel = await GetStoreItems();

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        public async Task<ActionResult> Popular()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            StoreViewModel viewModel = await GetStoreItems();

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        public async Task<ActionResult> Sale()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            StoreViewModel viewModel = await GetStoreItems();

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        private async Task<StoreViewModel> GetStoreItems()
        {
            StoreViewModel viewModel = new StoreViewModel();
            var storeItems = await storeService.GetStoreItemsAsync();

            var storeItemsViewModel = Mapper.Map<IReadOnlyList<StoreItem>, IReadOnlyList<StoreItemViewModel>>(storeItems);

            viewModel.StoreItems = storeItemsViewModel;
            viewModel.Spotlight = storeItemsViewModel[0];
            viewModel.GiftSpotlight = storeItemsViewModel[0];

            return viewModel;
        }

        [HttpPost]
        public async Task<JsonResult> BuyStoreItem(int storeItemId, int storeItemTypeId)
        {
            bool success = false;
            int count = 0;
            string currentUserId = User.Identity.GetUserId();

            try
            {
                if (storeItemTypeId == (int)StoreItemTypeValues.Gift)
                {
                    count = await storeService.BuyGiftAsync(currentUserId, storeItemId, 1);
                }
                else if (storeItemTypeId == (int)StoreItemTypeValues.Title)
                {
                    count = await storeService.BuyTitleAsync(currentUserId, storeItemId);
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error("BuyStoreItem", e,
                    parameters: new { storeItemId = storeItemId, currentUserId = currentUserId }
                );

                return Json(new { success = success, error = ErrorMessages.GiftPurchaseFailed });
            }

            success = count > 0;

            return Json(new { success = success, count = 1 });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (storeService != null)
                {
                    storeService.Dispose();
                    storeService = null;
                }

                if (userService != null)
                {
                    userService.Dispose();
                    userService = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}