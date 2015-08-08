using System;
using TwolipsDating.Utilities;

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

        public string TimeUntilSaleEnds 
        { 
            get 
            {
                return DateSaleEnds.HasValue ? DateSaleEnds.Value.GetTimeUntilEnd() : String.Empty;
            } 
        }

        public string DiscountPercent
        {
            get
            {
                return Discount.HasValue ? String.Format("-{0}%", Discount * 100) : String.Empty;
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