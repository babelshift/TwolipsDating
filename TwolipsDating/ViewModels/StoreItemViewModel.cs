using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwolipsDating.ViewModels
{
    public class StoreItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemImagePath { get; set; }
        public int PointsCost { get; set; }
        public int ItemTypeId { get; set; }
    }
}
