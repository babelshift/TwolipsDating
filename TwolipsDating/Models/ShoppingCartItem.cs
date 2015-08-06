﻿namespace TwolipsDating.Models
{
    public class ShoppingCartItem
    {
        public StoreItem Item { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get { return Item.PointPrice * Quantity; } }
    }
}