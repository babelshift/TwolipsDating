using System;

namespace TwolipsDating.ViewModels
{
    public class SpotlightSaleViewModel
    {
        public StoreItemViewModel StoreItem { get; set; }
        public double Discount { get; set; }
        public string TimeUntilEnd { get; set; }

        public string DiscountPercent { get { return String.Format("-{0}%", Discount * 100); } }
        public int DiscountedPointsCost { get { return (int)(StoreItem.PointsCost - (StoreItem.PointsCost * Discount)); } }
    }
}