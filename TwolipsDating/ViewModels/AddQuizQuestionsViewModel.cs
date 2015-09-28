using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class AddQuizQuestionsViewModel
    {
        public int QuizId { get; set; }

        public IList<CreateQuestionViewModel> Questions { get; set; }

        public IDictionary<int, string> Tags { get; set; }
    }
}