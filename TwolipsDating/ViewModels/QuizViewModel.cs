using System;
using System.Collections.Generic;
using System.Linq;

namespace TwolipsDating.ViewModels
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public IReadOnlyCollection<TagViewModel> Tags { get; set; }
        public bool IsAlreadyCompleted { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string ImageUrl { get; set; }
        public string SEOName { get; set; }

        public int AverageQuestionPoints
        {
            get
            {
                return (int)Math.Round(Questions.Average(x => x.Points));
            }
        }

        public int PointsPossible
        {
            get
            {
                return Questions.Sum(x => x.Points);
            }
        }

        #region User Already Completed Quiz

        public int UserScorePercent
        {
            get
            {
                return (int)Math.Round(((double)CorrectAnswerCount / (double)Questions.Count) * 100);
            }
        }

        public int CorrectAnswerCount
        {
            get
            {
                return Questions
                    .Where(x => x.CorrectAnswerId == x.SelectedAnswerId)
                    .Count();
            }
        }

        public int PointsEarned
        {
            get
            {
                return Questions
                    .Where(x => x.CorrectAnswerId == x.SelectedAnswerId)
                    .Sum(x => x.Points);
            }
        }

        #endregion User Already Completed Quiz

        public IReadOnlyCollection<UserCompletedQuizViewModel> UsersCompletedQuiz { get; set; }
        public IReadOnlyCollection<QuizOverviewViewModel> SimilarQuizzes { get; set; }
        public IReadOnlyCollection<QuizCategoryViewModel> QuizCategories { get; set; }
        public IReadOnlyCollection<UserWithSimilarQuizScoreViewModel> UsersWithSimilarScores { get; set; }

        public QuestionViolationViewModel QuestionViolation { get; set; }
    }
}