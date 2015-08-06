using System;

namespace TwolipsDating.Models
{
    public class CompletedQuiz
    {
        public int QuizId { get; set; }
        public string UserId { get; set; }
        public DateTime DateCompleted { get; set; }

        public virtual Quiz Quiz { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}