using System.Collections.Generic;
using System.Linq;

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