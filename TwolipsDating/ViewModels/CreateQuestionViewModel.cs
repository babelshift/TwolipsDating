using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class CreateQuestionViewModel
    {
        public string Content { get; set; }
        public int Points { get; set; }
        public IList<CreateAnswerViewModel> Answers { get; set; }
        public int CorrectAnswer { get; set; }
    }
}