using System;
using System.Collections.Generic;

namespace TwolipsDating.Models
{
    public class StoreItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ItemTypeId { get; set; }
        public int PointPrice { get; set; }
        public string IconFileName { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual StoreItemType ItemType { get; set; }
        public virtual ICollection<StoreTransactionLog> StoreTransactions { get; set; }
        public virtual ICollection<GiftTransactionLog> GiftTransactions { get; set; }
        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
        public virtual ICollection<UserTitle> OwnerUsers { get; set; }
        public virtual ICollection<Profile> SelectedByProfiles { get; set; }
    }
}