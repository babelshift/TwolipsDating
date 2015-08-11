using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class QuizSearchResultViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public int AveragePoints { get; set; }
        public int QuestionCount { get; set; }
    }
}