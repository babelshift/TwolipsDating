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
        public async Task<IReadOnlyCollection<Gift>> GetGiftsAsync()
        {
            var result = await (from gifts in db.Gifts
                                select gifts).ToListAsync();

            return result.AsReadOnly();
        }

        public async Task<int> BuyGiftAsync(string userId, int giftId, int buyCount)
        {
            // check price of item and see if the user has enough points to buy the item with the count
            var gift = await db.Gifts.FindAsync(giftId);
            var user = db.Users.Find(userId);

            if(user.Points >= gift.PointPrice * buyCount)
            {
                // check if user already has an item of this type
                var inventoryItem = await (from inventory in db.InventoryItems
                                           where inventory.GiftId == giftId
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
                        GiftId = giftId,
                        ItemCount = buyCount
                    };

                    db.InventoryItems.Add(item);
                }

                StoreTransactionLog log = new StoreTransactionLog()
                {
                    UserId = userId,
                    GiftId = giftId,
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