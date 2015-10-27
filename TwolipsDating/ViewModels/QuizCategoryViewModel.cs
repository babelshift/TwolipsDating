using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class QuizCategoryViewModel
    {
        public int QuizCategoryId { get; set; }
        public string QuizSEOName { get { return QuizExtensions.ToSEOName(QuizName); } }
        public string QuizIcon { get; set; }
        public string QuizName { get; set; }
    }
}
