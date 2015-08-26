using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class StoreController : BaseController
    {
        #region Services

        private StoreService storeService = new StoreService();
        private UserService userService = new UserService();

        #endregion Services

        /// <summary>
        /// Property exposing the shopping cart in the session.
        /// </summary>
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

        /// <summary>
        /// Returns a view model used to display the store of items.
        /// </summary>
        /// <returns></returns>
        [RequireProfile]
        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();

            await SetNotificationsAsync();

            var storeItems = await storeService.GetNewStoreItemsAsync();
            StoreViewModel viewModel = await GetStoreViewModelAsync(storeItems);

            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        ///// <summary>
        ///// Returns a view model used to display the store of items.
        ///// </summary>
        ///// <returns></returns>
        //public async Task<ActionResult> Popular()
        //{
        //    var currentUserId = User.Identity.GetUserId();
        //    if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

        //    await SetNotificationsAsync();

        //    var storeItems = await storeService.GetStoreItemsAsync();
        //    StoreViewModel viewModel = await GetStoreViewModelAsync(storeItems);

        //    viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

        //    return View(viewModel);
        //}

        ///// <summary>
        ///// Returns a view model used to display the store of items.
        ///// </summary>
        ///// <returns></returns>
        //public async Task<ActionResult> Sale()
        //{
        //    var currentUserId = User.Identity.GetUserId();
        //    if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

        //    await SetNotificationsAsync();

        //    var storeItems = await storeService.GetStoreItemsAsync();
        //    StoreViewModel viewModel = await GetStoreViewModelAsync(storeItems);

        //    viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

        //    return View(viewModel);
        //}

        /// <summary>
        /// Returns a view containing the user's shopping cart contents.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Cart()
        {
            await SetNotificationsAsync();

            var viewModel = Mapper.Map<ShoppingCart, ShoppingCartViewModel>(ShoppingCart);

            return View(viewModel);
        }

        /// <summary>
        /// Finishes the shopping phase for a user, checks them out, adds the items to their collection, and reduces their points.
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Checkout(ShoppingCartViewModel shoppingCart)
        {
            if (ModelState.IsValid)
            {
                List<int> shoppingCartItemsPurchased = new List<int>();

                foreach (var shoppingCartItem in shoppingCart.Items)
                {
                    // the user may have marked the item as removed from the cart in the UI, skip over those
                    if (!shoppingCartItem.IsRemoved)
                    {
                        bool successful = await BuyItem(shoppingCartItem.Item.ItemId, shoppingCartItem.Item.ItemTypeId, shoppingCartItem.Quantity);
                        if (successful)
                        {
                            shoppingCartItemsPurchased.Add(shoppingCartItem.Item.ItemId);
                        }
                    }
                }

                // remove only the items that were successfully purchased
                // this allows us to keep items in the cart that weren't purchased for whatever reason
                foreach(var shoppingCartItemId in shoppingCartItemsPurchased)
                {
                    ShoppingCart.RemoveItem(shoppingCartItemId);
                }
            }

            return RedirectToAction("index", "store");
        }

        /// <summary>
        /// Buys an item of a type and quantity for the currently logged in user.
        /// </summary>
        /// <param name="storeItemId"></param>
        /// <param name="storeItemTypeId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private async Task<bool> BuyItem(int storeItemId, int storeItemTypeId, int quantity)
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

            // if there were changes then the purchase was successful
            return count > 0;
        }

        /// <summary>
        /// Returns a view model consisting of all store items.
        /// </summary>
        /// <param name="storeItems"></param>
        /// <returns></returns>
        private async Task<StoreViewModel> GetStoreViewModelAsync(IReadOnlyList<StoreItemViewModel> storeItemViewModel)
        {
            StoreViewModel viewModel = new StoreViewModel();

            viewModel.StoreItems = storeItemViewModel;

            var spotlightSale = await storeService.GetCurrentSpotlightAsync();
            viewModel.Spotlight = Mapper.Map<StoreSale, StoreItemViewModel>(spotlightSale);

            var giftSpotlightSale = await storeService.GetCurrentGiftSpotlightAsync();
            viewModel.GiftSpotlight = Mapper.Map<StoreSale, StoreItemViewModel>(giftSpotlightSale);
            
            viewModel.ShoppingCartItemCount = ShoppingCart.Count;

            return viewModel;
        }

        /// <summary>
        /// Adds an item to the currently logged in user's cart.
        /// </summary>
        /// <param name="storeItemId"></param>
        /// <param name="storeItemTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddToCart(int storeItemId, int storeItemTypeId)
        {
            try
            {
                var storeItem = await storeService.GetStoreItemAsync(storeItemId);
                var storeItemViewModel = Mapper.Map<StoreItem, StoreItemViewModel>(storeItem);
                ShoppingCart.AddItem(storeItemViewModel);
                return Json(new { success = true, count = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, count = 1 });
            }
        }

        /// <summary>
        /// Removes an item from the currently logged in user's cart.
        /// </summary>
        /// <param name="storeItemId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveCartItem(int storeItemId)
        {
            ShoppingCart.RemoveItem(storeItemId);
            return Json(new { success = true });
        }

        /// <summary>
        /// Disposes all services.
        /// </summary>
        /// <param name="disposing"></param>
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