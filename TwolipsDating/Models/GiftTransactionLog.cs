using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class GiftTransactionLog
    {
        public int GiftTransactionLogId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public int StoreItemId { get; set; }
        public int ItemCount { get; set; }
        public DateTime DateTransactionOccurred { get; set; }
        public bool IsReviewedByToUser { get; set; }

        public virtual ApplicationUser FromUser { get; set; }
        public virtual ApplicationUser ToUser { get; set; }
        public virtual StoreItem StoreItem { get; set; }
    }
}