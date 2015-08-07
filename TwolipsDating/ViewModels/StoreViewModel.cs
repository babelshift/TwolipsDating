using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class StoreViewModel
    {
        public int ShoppingCartItemCount { get; set; }
        public SpotlightSaleViewModel Spotlight { get; set; }
        public SpotlightSaleViewModel GiftSpotlight { get; set; }
        public IReadOnlyCollection<StoreItemViewModel> StoreItems { get; set; }
        public bool IsCurrentUserEmailConfirmed { get; set; }
    }
}