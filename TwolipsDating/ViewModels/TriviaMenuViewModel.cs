using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class TriviaMenuViewModel
    {
        public IReadOnlyCollection<QuizOverviewViewModel> NewQuizzes { get; set; }
        public UserStatsViewModel UserStats { get; set; }
        public bool IsCurrentUserEmailConfirmed { get; set; }

        public IReadOnlyCollection<UserCompletedQuizViewModel> RecentlyCompletedQuizzes { get; set; }
    }
}