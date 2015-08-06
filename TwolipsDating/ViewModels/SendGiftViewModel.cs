using System.ComponentModel.DataAnnotations;

namespace TwolipsDating.ViewModels
{
    public class SendGiftViewModel
    {
        [Required]
        public int GiftId { get; set; }

        [Required]
        public int InventoryItemId { get; set; }
    }
}