using System.Collections.Generic;

namespace TwolipsDating.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
        public virtual ICollection<AnsweredQuestion> Instances { get; set; }
    }
}