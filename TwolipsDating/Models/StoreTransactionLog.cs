using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class StoreTransactionLog
    {
        public int StoreTransactionLogId { get; set; }
        public string UserId { get; set; }
        public int StoreItemId { get; set; }
        public int ItemCount { get; set; }
        public DateTime DateTransactionOccurred { get; set; }

        public virtual ApplicationUser BuyerUser { get; set; }
        public virtual StoreItem StoreItem { get; set; }
    }
}