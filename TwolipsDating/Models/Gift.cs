using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class Gift
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconFileName { get; set; }
        public int PointPrice { get; set; }

        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
        public virtual ICollection<GiftTransactionLog> GiftTransactions { get; set; }
        public virtual ICollection<StoreTransactionLog> StoreTransactions { get; set; }
    }
}