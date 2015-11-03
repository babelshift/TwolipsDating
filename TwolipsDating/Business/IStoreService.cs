using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwolipsDating.ViewModels;
namespace TwolipsDating.Business
{
    public interface IStoreService : IBaseService
    {
        System.Threading.Tasks.Task<ServiceResult> BuyGiftAsync(string userId, int storeItemId, int buyCount);
        System.Threading.Tasks.Task<ServiceResult> BuyTitleAsync(string userId, int storeItemId);
        System.Threading.Tasks.Task<TwolipsDating.Models.StoreSale> GetCurrentGiftSpotlightAsync();
        System.Threading.Tasks.Task<TwolipsDating.Models.StoreSale> GetCurrentSpotlightAsync();
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<TwolipsDating.ViewModels.StoreItemViewModel>> GetNewStoreItemsAsync();
        System.Threading.Tasks.Task<TwolipsDating.Models.StoreItem> GetStoreItemAsync(int storeItemId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<TwolipsDating.Models.StoreItem>> GetStoreItemsAsync();

        System.Threading.Tasks.Task<IReadOnlyCollection<RecentBuyerViewModel>> GetRecentBuyersAsync(string userId);
        Task<bool> IsActiveSaleAsync();
    }
}
