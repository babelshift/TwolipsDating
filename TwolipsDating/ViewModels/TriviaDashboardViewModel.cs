using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class TriviaDashboardViewModel
    {
        public IDictionary<int, IReadOnlyCollection<QuizOverviewViewModel>> DailyQuizzes { get; set; }
        public IReadOnlyCollection<QuizOverviewViewModel> NewQuizzes { get; set; }
        public IReadOnlyCollection<TrendingQuizViewModel> TrendingQuizzes { get; set; }
        public IReadOnlyCollection<MostPopularQuizViewModel> PopularQuizzes { get; set; }
        public IReadOnlyCollection<QuizOverviewViewModel> UnfinishedQuizzes { get; set; }
        public IReadOnlyCollection<UserCompletedQuizViewModel> RecentlyCompletedQuizzes { get; set; }
        public IReadOnlyCollection<QuizCategoryViewModel> QuizCategories { get; set; }
    }
}