using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public bool IsAlreadyCompleted { get; set; }

        public IReadOnlyCollection<UserCompletedQuizViewModel> UsersCompletedQuiz { get; set; }

        public QuestionViolationViewModel QuestionViolation { get; set; }
    }
}