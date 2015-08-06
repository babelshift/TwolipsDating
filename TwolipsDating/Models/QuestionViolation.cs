using System;

namespace TwolipsDating.Models
{
    public class QuestionViolation
    {
        public int Id { get; set; }
        public string AuthorUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public int QuestionId { get; set; }
        public int QuestionViolationTypeId { get; set; }

        public virtual ApplicationUser AuthorUser { get; set; }
        public virtual Question Question { get; set; }
        public virtual QuestionViolationType QuestionViolationType { get; set; }
    }
}