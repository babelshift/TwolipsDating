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
        public int? CorrectAnswerId { get; set; }
        public int Points { get; set; }
        public int? QuestionTypeId { get; set; }

        public virtual Answer CorrectAnswer { get; set; }
        public virtual QuestionType QuestionType { get; set; }

        public virtual ICollection<Answer> PossibleAnswers { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<AnsweredQuestion> AnsweredInstances { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}