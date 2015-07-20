using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class DashboardViewModel
    {
        public string CurrentUserId { get; set; }

        public bool IsCurrentUserEmailConfirmed { get; set; }
        public IReadOnlyList<DashboardItemViewModel> Items { get; set; }

        public WriteReviewViolationViewModel WriteReviewViolation { get; set; }

        public QuestionViewModel RandomQuestion { get; set; }
    }
}