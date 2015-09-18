using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public bool IsAlreadyCompleted { get; set; }
        public int AveragePoints { get; set; }
        public string ImageUrl { get; set; }
        public int UserScorePercent { get; set; }
        public string SEOName { get; set; }

        public IReadOnlyCollection<UserCompletedQuizViewModel> UsersCompletedQuiz { get; set; }
        public IReadOnlyCollection<QuizOverviewViewModel> SimilarQuizzes { get; set; }

        public QuestionViolationViewModel QuestionViolation { get; set; }

        public IReadOnlyCollection<TagViewModel> Tags { get; set; }

        public IReadOnlyCollection<QuizCategoryViewModel> QuizCategories { get; set; }
    }
}