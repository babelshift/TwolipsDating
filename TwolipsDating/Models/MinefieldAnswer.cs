using System.Collections.Generic;
namespace TwolipsDating.Models
{
    public class MinefieldAnswer
    {
        public int Id { get; set; }
        public int MinefieldQuestionId { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }

        public virtual MinefieldQuestion MinefieldQuestion { get; set; }
        public virtual ICollection<AnsweredMinefieldQuestion> Instances { get; set; }
    }
}