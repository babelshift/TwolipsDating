using TwolipsDating.ViewModels;
namespace TwolipsDating.Models
{
    public class ShoppingCartItem
    {
        public StoreItemViewModel Item { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get { return Item.DiscountedPointsCost * Quantity; } }
    }
}