using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UserPointsViewModel
    {
        public int PointsCount { get; set; }
        public int TotalSpent { get; set; }
        public IReadOnlyCollection<StoreTransactionViewModel> StoreTransactions { get; set; }
    }
}