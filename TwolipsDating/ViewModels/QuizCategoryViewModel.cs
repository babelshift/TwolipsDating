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
        public string QuizSEOName { get { return QuizExtensions.ToSEOName(QuizCategoryName); } }
        public string QuizIcon { get; set; }
        public string QuizCategoryName { get; set; }
    }
}
