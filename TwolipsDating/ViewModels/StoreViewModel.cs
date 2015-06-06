using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class StoreViewModel
    {
        public IReadOnlyCollection<StoreGiftViewModel> StoreGifts { get; set; }
        public IReadOnlyCollection<StoreTitleViewModel> StoreTitles { get; set; }
    }
}