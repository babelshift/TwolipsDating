using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class MinefieldQuestion
    {
        public int MinefieldQuestionId { get; set; }
        public string Content { get; set; }
        public int Points { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<MinefieldAnswer> PossibleAnswers { get; set; }
        public virtual ICollection<AnsweredMinefieldQuestion> AnsweredInstances { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}