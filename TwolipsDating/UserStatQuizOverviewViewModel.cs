using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating
{
    public class UserStatQuizOverviewViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailImagePath { get; set; }
        public int QuizCategoryId { get; set; }
        public string QuizCategoryName { get; set; }
        public string QuizCategorySEOName { get; set; }
        public int AveragePoints { get; set; }
        public int PointsEarned { get; set; }
        public int PointsPossible { get; set; }
        public int UserScorePercent
        {
            get
            {
                return (int)Math.Round(((double)CorrectAnswerCount / (double)PossibleCorrectAnswerCount) * 100);
            }
        }
        public int PossibleCorrectAnswerCount { get; set; }
        public int CorrectAnswerCount { get; set; }
        public string CompletedTimeAgo { get { return DateCompleted.GetTimeAgo(); } }
        public DateTime DateCompleted { get; set; }
    }
}