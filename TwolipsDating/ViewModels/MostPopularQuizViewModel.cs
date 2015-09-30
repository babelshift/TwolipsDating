using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class MostPopularQuizViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public int CompletedCount { get; set; }
        public string ThumbnailImagePath { get; set; }
    }
}