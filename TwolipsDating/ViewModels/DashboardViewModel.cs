using PagedList;
using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class DashboardViewModel
    {
        public string CurrentUserId { get; set; }

        public WriteReviewViolationViewModel WriteReviewViolation { get; set; }

        public QuestionViewModel RandomQuestion { get; set; }

        public IPagedList<DashboardItemViewModel> Items { get; set; }

        public IReadOnlyCollection<QuizOverviewViewModel> Quizzes { get; set; }

        public IReadOnlyCollection<PersonYouMightAlsoLikeViewModel> UsersToFollow { get; set; }
    }
}