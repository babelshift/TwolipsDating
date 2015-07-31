using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace TwolipsDating.Business
{
    public class StoreService : BaseService
    {
        internal async Task<StoreItem> GetStoreItemAsync(int storeItemId)
        {
            return await db.StoreItems.FindAsync(storeItemId);
        }

        internal async Task<IReadOnlyList<StoreItem>> GetStoreItemsAsync()
        {
            var result = await (from storeItems in db.StoreItems
                                select storeItems).ToListAsync();

            return result.AsReadOnly();
        }

        internal async Task<IReadOnlyList<StoreItem>> GetNewStoreItemsAsync()
        {
            var result = await (from storeItems in db.StoreItems
                                orderby storeItems.DateAdded descending
                                select storeItems).ToListAsync();

            return result.AsReadOnly();
        }

        //public async Task<IReadOnlyCollection<Gift>> GetGiftsAsync()
        //{
        //    var result = await (from gifts in db.Gifts
        //                        select gifts).ToListAsync();

        //    return result.AsReadOnly();
        //}

        //internal async Task<IReadOnlyCollection<Title>> GetTitlesAsync()
        //{
        //    var result = await (from titles in db.Titles
        //                        select titles).ToListAsync();

        //    return result.AsReadOnly();
        //}



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

    }
}