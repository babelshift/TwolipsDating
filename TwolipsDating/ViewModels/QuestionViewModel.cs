using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TwolipsDating.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public int Points { get; set; }
        public int QuestionTypeId { get; set; }

        public bool IsAlreadyAnswered { get; set; }

        [Required(ErrorMessage = "You have to select an answer to this question.")]
        public int? SelectedAnswerId { get; set; }

        public int CorrectAnswerId { get; set; }

        public IReadOnlyCollection<AnswerViewModel> Answers { get; set; }

        public IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel> UsersAnsweredCorrectly { get; set; }

        public QuestionViolationViewModel QuestionViolation { get; set; }

        public IReadOnlyCollection<TagViewModel> Tags { get; set; }
    }
}