using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwolipsDating.ViewModels
{
    public class MinefieldQuestionViewModel
    {
        public int MinefieldQuestionId { get; set; }
        public string Content { get; set; }
        public int Points { get; set; }

        public bool IsAlreadyAnswered { get; set; }

        public List<MinefieldAnswerViewModel> Answers { get; set; }

        public IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel> UsersAnsweredCorrectly { get; set; }

        public QuestionViolationViewModel QuestionViolation { get; set; }

        public IReadOnlyCollection<TagViewModel> Tags { get; set; }
    }
}
