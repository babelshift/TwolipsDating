using System.Collections.Generic;

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
        public virtual ICollection<QuestionViolation> QuestionViolations { get; set; }
    }
}