using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class CompletedQuizFeedViewModel
    {
        public string SourceProfileId { get; set; }
        public string SourceUserName { get; set; }
        public string SourceProfileSEOName
        {
            get
            {
                return ProfileExtensions.GetProfileSEOName(SourceUserName);
            }
        }
        public string SourceProfileImagePath { get; set; }
        public int CorrectAnswerCount { get; set; }
        public int TotalAnswerCount { get; set; }
        public int UserScorePercent { get { return (int)Math.Round(((double)CorrectAnswerCount / (double)TotalAnswerCount) * 100); } }
        public string QuizName { get; set; }
        public string QuizSEOName
        {
            get
            {
                return QuizExtensions.GetQuizSEOName(QuizName);
            }
        }
        public int QuizId { get; set; }
        public string TimeAgo { get { return DateCompleted.GetTimeAgo(); } }
        public DateTime DateCompleted { get; set; }
        public string QuizThumbnailImagePath { get; set; }
    }
}