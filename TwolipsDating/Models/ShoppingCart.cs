using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class ShoppingCart
    {
        private List<ShoppingCartItem> items = new List<ShoppingCartItem>();

        public IReadOnlyList<ShoppingCartItem> Items
        {
            get
            {
                return items.AsReadOnly();
            }
        }

        public int TotalCost
        {
            get
            {
                return items.Sum(t => t.TotalPrice);
            }
        }

        public void AddItem(StoreItem item)
        {
            // optimize this with a hash table lookup
            var match = items.FirstOrDefault(i => i.Item.Id == item.Id);

            if(match == null)
            {
                ShoppingCartItem cartItem = new ShoppingCartItem()
                {
                    Item = item,
                    Quantity = 1
                };
                items.Add(cartItem);
            }
            else
            {
                match.Quantity++;
            }
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}