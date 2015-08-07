using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class DashboardViewModel
    {
        public string CurrentUserId { get; set; }

        public bool IsCurrentUserEmailConfirmed { get; set; }

        public WriteReviewViolationViewModel WriteReviewViolation { get; set; }

        public QuestionViewModel RandomQuestion { get; set; }

        public IReadOnlyList<DashboardItemViewModel> Items { get; set; }

        public IReadOnlyCollection<QuizOverviewViewModel> Quizzes { get; set; }

        public IReadOnlyCollection<ProfileViewModel> UsersToFollow { get; set; }
    }
}