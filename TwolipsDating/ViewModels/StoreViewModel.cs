using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class StoreViewModel
    {
        public StoreItemViewModel Spotlight { get; set; }
        public StoreItemViewModel GiftSpotlight { get; set; }
        public IReadOnlyCollection<StoreItemViewModel> StoreItems { get; set; }
        public bool IsCurrentUserEmailConfirmed { get; set; }
    }
}