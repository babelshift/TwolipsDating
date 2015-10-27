using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class TrendingQuizViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public string QuizSEOName
        {
            get
            {
                return QuizExtensions.ToSEOName(QuizName);
            }
        }
        public int CompletedCount { get; set; }
        public int PercentageOfTrending { get; set; }
        public string ThumbnailImagePath { get; set; }
    }
}