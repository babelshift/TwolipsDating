using PagedList;
using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class UserPointsViewModel
    {
        public int PointsCount { get; set; }
        public int TotalSpent { get; set; }
        public IPagedList<StoreTransactionViewModel> StoreTransactions { get; set; }
        public bool IsCurrentUserEmailConfirmed { get; set; }
    }
}