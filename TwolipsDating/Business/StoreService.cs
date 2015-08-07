using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class StoreService : BaseService
    {
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
        internal async Task<IReadOnlyList<StoreItem>> GetNewStoreItemsAsync()
        {
            var result = await (from storeItems in db.StoreItems
                                orderby storeItems.DateAdded descending
                                select storeItems).ToListAsync();

            return result.AsReadOnly();
        }

        /// <summary>
        /// Adds a title to a user's collection and reduces their points appropriately.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="storeItemId"></param>
        /// <returns></returns>
        public async Task<int> BuyTitleAsync(string userId, int storeItemId)
        {
            var title = await db.StoreItems.FindAsync(storeItemId);
            var user = db.Users.Find(userId);

            if (user.Points >= title.PointPrice)
            {
                UserTitle userTitle = new UserTitle()
                {
                    UserId = userId,
                    StoreItemId = storeItemId,
                    DateObtained = DateTime.Now
                };

                db.UserTitles.Add(userTitle);

                user.Points -= title.PointPrice;
            }

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a gift to a user's collection and reduces their points appropriately.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="storeItemId"></param>
        /// <param name="buyCount"></param>
        /// <returns></returns>
        public async Task<int> BuyGiftAsync(string userId, int storeItemId, int buyCount)
        {
            // check price of item and see if the user has enough points to buy the item with the count
            var gift = await db.StoreItems.FindAsync(storeItemId);
            var user = db.Users.Find(userId);

            if (user.Points >= gift.PointPrice * buyCount)
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

                StoreTransactionLog log = new StoreTransactionLog()
                {
                    UserId = userId,
                    StoreItemId = storeItemId,
                    ItemCount = buyCount,
                    DateTransactionOccurred = DateTime.Now
                };

                db.StoreTransactions.Add(log);

                user.Points -= gift.PointPrice * buyCount;
            }

            return await db.SaveChangesAsync();
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