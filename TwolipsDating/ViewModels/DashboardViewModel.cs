using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class DashboardViewModel
    {
        public bool IsCurrentUserEmailConfirmed { get; set; }
        public IReadOnlyList<DashboardItemViewModel> Items { get; set; }
    }
}