using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItemViewModel> Items { get; set; }

        public int TotalCost 
        { 
            get 
            { 
                return Items != null ? Items.Sum(t => t.TotalPrice) : 0; 
            } 
        }
    }
}