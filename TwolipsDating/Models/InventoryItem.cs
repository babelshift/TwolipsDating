using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }

        [IndexAttribute("UX_OwnerAndGift", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string ApplicationUserId { get; set; }

        [IndexAttribute("UX_OwnerAndGift", 2, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int StoreItemId { get; set; }

        public int ItemCount { get; set; }

        public virtual StoreItem StoreItem { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
    }
}