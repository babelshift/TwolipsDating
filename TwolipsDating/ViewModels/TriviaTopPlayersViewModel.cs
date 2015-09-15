using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class TriviaTopPlayersViewModel
    {
        public IReadOnlyCollection<ProfileViewModel> Players { get; set; }

        public IReadOnlyCollection<TrendingQuizViewModel> TrendingQuizzes { get; set; }
        public IReadOnlyCollection<MostPopularQuizViewModel> PopularQuizzes { get; set; }
        public IReadOnlyCollection<UserCompletedQuizViewModel> RecentlyCompletedQuizzes { get; set; }
        public IReadOnlyCollection<QuizCategoryViewModel> QuizCategories { get; set; }
    }
}