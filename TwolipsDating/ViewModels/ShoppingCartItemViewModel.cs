﻿namespace TwolipsDating.ViewModels
{
    public class ShoppingCartItemViewModel
    {
        public StoreItemViewModel Item { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get { return Quantity * Item.DiscountedPointsCost; } }
        public bool IsRemoved { get; set; }
    }
}