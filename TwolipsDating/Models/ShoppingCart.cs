﻿using System.Collections.Generic;
using System.Linq;
using TwolipsDating.ViewModels;

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

        public int Count
        {
            get
            {
                int count = 0;
                foreach (var item in items)
                {
                    count += item.Quantity;
                }
                return count;
            }
        }

        public int TotalCost
        {
            get
            {
                return items.Sum(t => t.TotalPrice);
            }
        }

        public void AddItem(StoreItemViewModel item)
        {
            // optimize this with a hash table lookup
            var match = items.FirstOrDefault(i => i.Item.ItemId == item.ItemId);

            if (match == null)
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

        public void RemoveItem(int storeItemId)
        {
            var match = items.FirstOrDefault(i => i.Item.ItemId == storeItemId);
            if (match != null)
            {
                items.Remove(match);
            }
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}