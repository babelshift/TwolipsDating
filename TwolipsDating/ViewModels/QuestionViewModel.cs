using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }

        public int SelectedAnswerId { get; set; }

        public IReadOnlyCollection<AnswerViewModel> Answers { get; set; }
    }
}