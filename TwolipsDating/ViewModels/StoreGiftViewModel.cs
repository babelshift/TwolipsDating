using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class StoreGiftViewModel
    {
        public int GiftId { get; set; }
        public string GiftImagePath { get; set; }
        public int PointPrice { get; set; }
        public string GiftName { get; set; }
        public string GiftDescription { get; set; }
    }
}