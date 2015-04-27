using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int? QuizId { get; set; }
        public int? CorrectAnswerId { get; set; }
        public int Points { get; set; }

        public virtual Quiz Quiz { get; set; }
        public virtual Answer CorrectAnswer { get; set; }

        public virtual ICollection<Answer> PossibleAnswers { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<AnsweredQuestion> AnsweredInstances { get; set; }
    }
}