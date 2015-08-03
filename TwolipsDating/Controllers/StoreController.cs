﻿using AutoMapper;
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

        private ShoppingCart ShoppingCart
        {
            get
            {
                if (Session["ShoppingCart"] == null)
                {
                    Session["ShoppingCart"] = new ShoppingCart();
                }

                return (ShoppingCart)Session["ShoppingCart"];
            }
            set
            {
                if (value != null)
                {
                    Session["ShoppingCart"] = (ShoppingCart)value;
                }
            }
        }

        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            var storeItems = await storeService.GetNewStoreItemsAsync();
            StoreViewModel viewModel = GetStoreItemViewModel(storeItems);

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        public async Task<ActionResult> Popular()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            var storeItems = await storeService.GetStoreItemsAsync();
            StoreViewModel viewModel = GetStoreItemViewModel(storeItems);

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        public async Task<ActionResult> Sale()
        {
            var currentUserId = User.Identity.GetUserId();
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            await SetNotificationsAsync();

            var storeItems = await storeService.GetStoreItemsAsync();
            StoreViewModel viewModel = GetStoreItemViewModel(storeItems);

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        public async Task<ActionResult> Cart()
        {
            await SetNotificationsAsync();

            var viewModel = Mapper.Map<ShoppingCart, ShoppingCartViewModel>(ShoppingCart);

            return View(viewModel);
        }

        public async Task<ActionResult> Checkout(ShoppingCartViewModel shoppingCart)
        {
            if(ModelState.IsValid)
            {
                foreach(var item in shoppingCart.Items)
                {
                    await BuyItem(item.Item.ItemId, item.Item.ItemTypeId, item.Quantity);
                }

                ShoppingCart.Clear();
            }

            return RedirectToAction("index", "store");
        }

        private async Task BuyItem(int storeItemId, int storeItemTypeId, int quantity)
        {
            int count = 0;
            string currentUserId = User.Identity.GetUserId();

            try
            {
                if (storeItemTypeId == (int)StoreItemTypeValues.Gift)
                {
                    count = await storeService.BuyGiftAsync(currentUserId, storeItemId, quantity);
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
            }
        }

        private StoreViewModel GetStoreItemViewModel(IReadOnlyList<StoreItem> storeItems)
        {
            StoreViewModel viewModel = new StoreViewModel();

            var storeItemsViewModel = Mapper.Map<IReadOnlyList<StoreItem>, IReadOnlyList<StoreItemViewModel>>(storeItems);

            viewModel.StoreItems = storeItemsViewModel;
            viewModel.Spotlight = storeItemsViewModel[0];
            viewModel.GiftSpotlight = storeItemsViewModel[1];
            viewModel.ShoppingCartItemCount = ShoppingCart.Count;

            return viewModel;
        }

        [HttpPost]
        public async Task<JsonResult> BuyStoreItem(int storeItemId, int storeItemTypeId)
        {
            try
            {
                var storeItem = await storeService.GetStoreItemAsync(storeItemId);
                ShoppingCart.AddItem(storeItem);
                return Json(new { success = true, count = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, count = 1 });
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveCartItem(int storeItemId)
        {
            ShoppingCart.RemoveItem(storeItemId);
            return Json(new { success = true });
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