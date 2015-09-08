using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
using TwolipsDating.Utilities;
using System.Data.Entity.Infrastructure;

namespace TwolipsDating.Business
{
    public class StoreService : BaseService
    {
        public StoreService(IValidationDictionary validationDictionary)
            : base(validationDictionary) { }

        /// <summary>
        /// Returns a store item.
        /// </summary>
        /// <param name="storeItemId"></param>
        /// <returns></returns>
        internal async Task<StoreItem> GetStoreItemAsync(int storeItemId)
        {
            return await db.StoreItems.FindAsync(storeItemId);
        }

        /// <summary>
        /// Returns all store items.
        /// </summary>
        /// <returns></returns>
        internal async Task<IReadOnlyList<StoreItem>> GetStoreItemsAsync()
        {
            var result = await (from storeItems in db.StoreItems
                                select storeItems).ToListAsync();

            return result.AsReadOnly();
        }

        /// <summary>
        /// Returns all store items in descending order by the date at which they were added.
        /// </summary>
        /// <returns></returns>
        internal async Task<IReadOnlyList<StoreItemViewModel>> GetNewStoreItemsAsync()
        {
            DateTime now = DateTime.Now;

            var query = from storeItems in db.StoreItems
                        join sales in db.StoreSales on storeItems.Id equals sales.StoreItemId into lj
                        from sales in lj.DefaultIfEmpty()
                        where (now >= sales.DateStart && now <= sales.DateEnd) || sales.Discount == null // sales for today or items without sales
                        orderby storeItems.DateAdded descending
                        select new StoreItemViewModel()
                        {
                            ItemId = storeItems.Id,
                            Discount = sales.Discount,
                            ItemDescription = storeItems.Description,
                            ItemName = storeItems.Name,
                            ItemTypeId = storeItems.ItemTypeId,
                            PointsCost = storeItems.PointPrice,
                            ItemImagePath = storeItems.IconFileName,
                            DateSaleEnds = sales.DateEnd
                        };

            var results = await query.ToListAsync();

            // we couldn't execute these custom formatting functions in the LINQ query, so we loop and do it here
            foreach (var item in results)
            {
                item.ItemImagePath = item.GetIconPath();
            }

            return results;
        }

        /// <summary>
        /// Adds a title to a user's collection and reduces their points appropriately.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="storeItemId"></param>
        /// <returns></returns>
        public async Task<ServiceResult> BuyTitleAsync(string userId, int storeItemId)
        {
            var title = await db.StoreItems.FindAsync(storeItemId);
            var user = db.Users.Find(userId);

            // get any sales that this item is under
            int pointsCost = await GetAdjustedSalePrice(storeItemId, title.PointPrice);

            bool success = false;
            string failMessage = String.Empty;

            if (user.Points >= pointsCost)
            {
                try
                {
                    UserTitle userTitle = new UserTitle()
                    {
                        UserId = userId,
                        StoreItemId = storeItemId,
                        DateObtained = DateTime.Now
                    };

                    db.UserTitles.Add(userTitle);

                    LogStoreTransaction(userId, storeItemId, 1, pointsCost);

                    user.Points -= pointsCost;

                    success = (await db.SaveChangesAsync()) > 0;
                }
                catch (DbUpdateException ex)
                {
                    Log.Error("StoreService.BuyGiftAsync", ex, new { userId, storeItemId });
                    ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.TitlePurchaseFailed);
                }

                if (success)
                {
                    await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.TitlesPurchased);
                }
                else
                {
                    failMessage = ErrorMessages.TitlePurchaseFailed;
                }
            }
            else
            {
                failMessage = String.Format(ErrorMessages.UserDoesNotHaveEnoughPointsToPurchase, 1, title.Name);
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), failMessage);
            }

            return success ? ServiceResult.Success : ServiceResult.Failed(failMessage);
        }

        /// <summary>
        /// Adds a gift to a user's collection and reduces their points appropriately.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="storeItemId"></param>
        /// <param name="buyCount"></param>
        /// <returns></returns>
        public async Task<ServiceResult> BuyGiftAsync(string userId, int storeItemId, int buyCount)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(storeItemId > 0);
            Debug.Assert(buyCount > 0);

            // check price of item and see if the user has enough points to buy the item with the count
            var gift = await db.StoreItems.FindAsync(storeItemId);
            var user = db.Users.Find(userId);

            // get any sales that this item is under
            int pointsCost = await GetAdjustedSalePrice(storeItemId, gift.PointPrice);

            bool success = false;
            string failMessage = String.Empty;

            if (user.Points >= pointsCost * buyCount)
            {
                try
                {
                    // check if user already has an item of this type
                    var inventoryItem = await (from inventory in db.InventoryItems
                                               where inventory.StoreItemId == storeItemId
                                               where inventory.ApplicationUserId == userId
                                               select inventory).FirstOrDefaultAsync();

                    // if they do, increase the count of their item based on buyCount
                    if (inventoryItem != null)
                    {
                        inventoryItem.ItemCount += buyCount;
                    }
                    // if they don't, create a new item and add it to their inventory
                    else
                    {
                        InventoryItem item = new InventoryItem()
                        {
                            ApplicationUserId = userId,
                            StoreItemId = storeItemId,
                            ItemCount = buyCount
                        };

                        db.InventoryItems.Add(item);
                    }

                    LogStoreTransaction(userId, storeItemId, buyCount, pointsCost);

                    user.Points -= pointsCost * buyCount;

                    success = (await db.SaveChangesAsync()) > 0;
                }
                catch (DbUpdateException ex)
                {
                    Log.Error("StoreService.BuyGiftAsync", ex, new { userId, storeItemId, buyCount });
                    ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.GiftPurchaseFailed);
                }

                if (success)
                {
                    await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.GiftsPurchased);
                }
                else
                {
                    failMessage = ErrorMessages.GiftPurchaseFailed;
                }

            }
            else
            {
                failMessage = String.Format(ErrorMessages.UserDoesNotHaveEnoughPointsToPurchase, buyCount, gift.Name);
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), failMessage);
            }

            return success ? ServiceResult.Success : ServiceResult.Failed(failMessage);
        }

        private void LogStoreTransaction(string userId, int storeItemId, int buyCount, int pointPrice)
        {
            StoreTransactionLog log = new StoreTransactionLog()
            {
                UserId = userId,
                StoreItemId = storeItemId,
                ItemCount = buyCount,
                PointPrice = pointPrice,
                DateTransactionOccurred = DateTime.Now
            };

            db.StoreTransactions.Add(log);
        }

        private async Task<int> GetAdjustedSalePrice(int storeItemId, int originalPointsPrice)
        {
            int salePointsPrice = originalPointsPrice;

            DateTime now = DateTime.Now;

            var saleForItem = await (from sales in db.StoreSales
                                     where sales.StoreItemId == storeItemId
                                     where now >= sales.DateStart && now <= sales.DateEnd
                                     select sales).FirstOrDefaultAsync();

            if (saleForItem != null)
            {
                salePointsPrice -= (int)Math.Round(salePointsPrice * saleForItem.Discount);
            }

            return salePointsPrice;
        }

        internal async Task<StoreSale> GetCurrentSpotlightAsync()
        {
            DateTime now = DateTime.Now;

            var currentSpotlightSale = await (from spotlights in db.StoreSpotlights
                                              where now >= spotlights.DateStart && now <= spotlights.DateEnd
                                              select spotlights.StoreSale).FirstOrDefaultAsync();

            return currentSpotlightSale;
        }

        internal async Task<StoreSale> GetCurrentGiftSpotlightAsync()
        {
            DateTime now = DateTime.Now;

            var currentSpotlightSale = await (from spotlights in db.StoreGiftSpotlights
                                              where now >= spotlights.DateStart && now <= spotlights.DateEnd
                                              select spotlights.StoreSale).FirstOrDefaultAsync();

            return currentSpotlightSale;
        }
    }
}