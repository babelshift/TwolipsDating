using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class StoreService : BaseService, IStoreService
    {
        public StoreService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        internal static StoreService Create(IdentityFactoryOptions<StoreService> options, IOwinContext context)
        {
            var service = new StoreService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

        /// <summary>
        /// Returns a store item.
        /// </summary>
        /// <param name="storeItemId"></param>
        /// <returns></returns>
        public async Task<StoreItem> GetStoreItemAsync(int storeItemId)
        {
            return await db.StoreItems.FindAsync(storeItemId);
        }

        /// <summary>
        /// Returns all store items.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<StoreItem>> GetStoreItemsAsync()
        {
            var result = await (from storeItems in db.StoreItems
                                select storeItems).ToListAsync();

            return result.AsReadOnly();
        }

        /// <summary>
        /// Returns all store items in descending order by the date at which they were added.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<StoreItemViewModel>> GetNewStoreItemsAsync()
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
                item.ItemImagePath = StoreItemExtensions.GetImagePath(item.ItemImagePath);
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
            List<string> errors = new List<string>();

            if (user.CurrentPoints >= pointsCost)
            {
                try
                {
                    UserTitle userTitle = new UserTitle()
                    {
                        UserId = userId,
                        StoreItemId = storeItemId,
                        DateObtained = DateTime.Now
                    };

                    // if the user does not own the title, let them purchase it
                    bool doesUserAlreadyHaveTitle = await db.UserTitles.AnyAsync(a => a.StoreItemId == storeItemId && a.UserId == userId);
                    if (!doesUserAlreadyHaveTitle)
                    {
                        db.UserTitles.Add(userTitle);

                        LogStoreTransaction(userId, storeItemId, 1, pointsCost);

                        user.CurrentPoints -= pointsCost;

                        success = (await db.SaveChangesAsync()) > 0;
                    }
                    else
                    {
                        errors.Add(ErrorMessages.TitleAlreadyObtained);
                        ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.TitleAlreadyObtained);
                    }
                }
                catch (DbUpdateException ex)
                {
                    Log.Error("StoreService.BuyGiftAsync", ex, new { userId, storeItemId });
                    errors.Add(ErrorMessages.TitleAlreadyObtained);
                }

                if (success)
                {
                    await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.TitlesPurchased);
                }
            }
            else
            {
                string error = String.Format(ErrorMessages.UserDoesNotHaveEnoughPointsToPurchase, 1, title.Name);
                errors.Add(error);
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), error);
            }

            return success ? ServiceResult.Success : ServiceResult.Failed(errors.ToArray());
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

            if (user.CurrentPoints >= pointsCost * buyCount)
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

                    user.CurrentPoints -= pointsCost * buyCount;

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

        public async Task<StoreSale> GetCurrentSpotlightAsync()
        {
            DateTime now = DateTime.Now;

            var currentSpotlightSale = await (from spotlights in db.StoreSpotlights
                                              where now >= spotlights.DateStart && now <= spotlights.DateEnd
                                              select spotlights.StoreSale).FirstOrDefaultAsync();

            return currentSpotlightSale;
        }

        public async Task<StoreSale> GetCurrentGiftSpotlightAsync()
        {
            DateTime now = DateTime.Now;

            var currentSpotlightSale = await (from spotlights in db.StoreGiftSpotlights
                                              where now >= spotlights.DateStart && now <= spotlights.DateEnd
                                              select spotlights.StoreSale).FirstOrDefaultAsync();

            return currentSpotlightSale;
        }

        public async Task<IReadOnlyCollection<RecentBuyerViewModel>> GetRecentBuyersAsync(string userId)
        {
            var recentBuyers = await (from purchases in db.StoreTransactions
                                      where purchases.BuyerUser.IsActive
                                      group purchases by new
                                      {
                                          Birthday = purchases.BuyerUser.Profile.Birthday,
                                          CityName = purchases.BuyerUser.Profile.GeoCity.Name,
                                          ProfileId = purchases.BuyerUser.Profile.Id,
                                          ProfileImagePath = purchases.BuyerUser.Profile.UserImage.FileName,
                                          StateName = purchases.BuyerUser.Profile.GeoCity.GeoState.Abbreviation,
                                          UserId = purchases.BuyerUser.Id,
                                          UserName = purchases.BuyerUser.UserName,
                                          IsFavoritedByCurrentUser = purchases.BuyerUser.Profile.FavoritedBy.Any(x => x.UserId == userId),
                                      } into g
                                      select new
                                      {
                                          Birthday = g.Key.Birthday,
                                          CityName = g.Key.CityName,
                                          ProfileId = g.Key.ProfileId,
                                          ProfileImagePath = g.Key.ProfileImagePath,
                                          StateName = g.Key.StateName,
                                          UserId = g.Key.UserId,
                                          UserName = g.Key.UserName,
                                          IsFavoritedByCurrentUser = g.Key.IsFavoritedByCurrentUser,
                                          DatePurchased = g.Max(x => x.DateTransactionOccurred)
                                      })
                                      .OrderByDescending(x => x.DatePurchased)
                                      .Select(x => new RecentBuyerViewModel()
                                      {
                                          Birthday = x.Birthday,
                                          CityName = x.CityName,
                                          ProfileId = x.ProfileId,
                                          ProfileImagePath = x.ProfileImagePath,
                                          StateName = x.StateName,
                                          UserId = x.UserId,
                                          UserName = x.UserName,
                                          IsFavoritedByCurrentUser = x.IsFavoritedByCurrentUser
                                      })
                                      .Take(10)
                                      .ToListAsync();

            foreach (var recentBuyer in recentBuyers)
            {
                recentBuyer.ProfileImagePath = ProfileExtensions.GetThumbnailImagePath(recentBuyer.ProfileImagePath);
                recentBuyer.Age = DateTimeExtensions.GetAge(recentBuyer.Birthday);
                recentBuyer.Location = CityExtensions.ToFullLocationString(recentBuyer.CityName, recentBuyer.StateName);
            }

            return recentBuyers.AsReadOnly();
        }
    }
}