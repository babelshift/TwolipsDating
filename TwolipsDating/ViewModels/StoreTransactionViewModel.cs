using System;

namespace TwolipsDating.ViewModels
{
    public class StoreTransactionViewModel
    {
        public DateTime TransactionDate { get; set; }
        public string ItemName { get; set; }
        public int ItemCount { get; set; }
        public int ItemCost { get; set; }
        public int TotalCost { get; set; }
        public string ItemType { get; set; }
    }
}