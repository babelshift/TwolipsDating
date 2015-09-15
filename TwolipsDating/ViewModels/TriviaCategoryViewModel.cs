using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class TriviaCategoryViewModel
    {
        public int ActiveQuizCategoryId { get; set; }
        public string ActiveQuizCategoryName { get; set; }
        public string ActiveQuizCategoryIcon { get; set; }

        public IReadOnlyCollection<QuizOverviewViewModel> Quizzes { get; set; }
        public IReadOnlyCollection<TrendingQuizViewModel> TrendingQuizzes { get; set; }
        public IReadOnlyCollection<MostPopularQuizViewModel> PopularQuizzes { get; set; }
        public IReadOnlyCollection<UserCompletedQuizViewModel> RecentlyCompletedQuizzes { get; set; }
        public IReadOnlyCollection<QuizCategoryViewModel> QuizCategories {  get; set; }
    }
}