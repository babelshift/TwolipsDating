using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class NotificationsViewModel
    {
        public IPagedList<DashboardItemViewModel> Items { get; set; }

        public IReadOnlyCollection<PersonYouMightAlsoLikeViewModel> UsersToFollow { get; set; }
    }
}