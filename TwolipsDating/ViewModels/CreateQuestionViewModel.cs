using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class CreateQuestionViewModel
    {
        [Required]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Points { get; set; }

        [Range(1, 4)]
        public int CorrectAnswer { get; set; }

        public IList<CreateAnswerViewModel> Answers { get; set; }

        public List<int> SelectedTags { get; set; }
    }
}