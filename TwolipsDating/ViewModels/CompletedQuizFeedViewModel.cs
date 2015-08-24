using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class CompletedQuizFeedViewModel
    {
        public string SourceProfileId { get; set; }
        public string SourceUserName { get; set; }
        public string SourceProfileImagePath { get; set; }
        public int CorrectAnswerCount { get; set; }
        public int TotalAnswerCount { get; set; }
        public int UserScorePercent { get { return (int)Math.Round(((double)CorrectAnswerCount / (double)TotalAnswerCount) * 100); } }
        public string QuizName { get; set; }
        public int QuizId { get; set; }
        public string TimeAgo { get; set; }
        public DateTime DateCompleted { get; set; }
    }
}