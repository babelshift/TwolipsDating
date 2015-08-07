using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class StoreSale
    {
        public int SaleId { get; set; }

        [IndexAttribute("UX_StoreItemAndDate", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int StoreItemId { get; set; }

        public double Discount { get; set; }

        [IndexAttribute("UX_StoreItemAndDate", 2, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public DateTime DateStart { get; set; }

        [IndexAttribute("UX_StoreItemAndDate", 3, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public DateTime DateEnd { get; set; }

        public virtual StoreItem StoreItem { get; set; }

        public virtual ICollection<StoreSpotlight> StoreSpotlights { get; set; }
        public virtual ICollection<StoreGiftSpotlight> StoreGiftSpotlights { get; set; }
    }
}