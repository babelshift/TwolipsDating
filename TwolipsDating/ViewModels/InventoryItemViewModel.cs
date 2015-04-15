using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class InventoryItemViewModel
    {
        public int InventoryItemId { get; set; }
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public string GiftDescription { get; set; }
        public int ItemCount { get; set; }
        public string GiftIconFilePath { get; set; }
    }
}