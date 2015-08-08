using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class StoreViewModel
    {
        public int ShoppingCartItemCount { get; set; }
        public StoreItemViewModel Spotlight { get; set; }
        public StoreItemViewModel GiftSpotlight { get; set; }
        public IReadOnlyCollection<StoreItemViewModel> StoreItems { get; set; }
        public bool IsCurrentUserEmailConfirmed { get; set; }
    }
}