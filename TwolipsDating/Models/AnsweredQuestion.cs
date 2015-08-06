using System;

namespace TwolipsDating.Models
{
    public class AnsweredQuestion
    {
        public string UserId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public DateTime DateAnswered { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; set; }
    }
}