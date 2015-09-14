using System;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class UserCompletedQuizViewModel
    {
        public string QuizName { get; set; }
        public string ProfileImagePath { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int ProfileId { get; set; }
        public DateTime DateCompleted { get; set; }
        public string TimeAgo { get { return DateCompleted.GetTimeAgo(); } }
        public int CorrectAnswerCount { get; set; }
        public int TotalAnswerCount { get; set; }
        public int UserScorePercent { get { return (int)Math.Round(((double)CorrectAnswerCount / (double)TotalAnswerCount) * 100); } }
        public bool IsFavoritedByCurrentUser { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Location { get; set; }
    }
}