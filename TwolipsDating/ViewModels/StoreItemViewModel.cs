using System;

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
        public double? Discount { get; set; } // not all items are on sale
        public DateTime? DateSaleEnds { get; set; } // not all items are on sale
        public string TimeUntilSaleEnds { get; set; }

        public string DiscountPercent
        {
            get
            {
                string percent = Discount.HasValue ? String.Format("-{0}%", Discount * 100) : String.Empty;
                return percent;
            }
        }

        public int DiscountedPointsCost
        {
            get
            {
                return Discount.HasValue ? (int)(PointsCost - (PointsCost * Discount)) : PointsCost;
            }
        }
    }
}