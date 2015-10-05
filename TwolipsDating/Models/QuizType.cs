using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public enum QuizTypeValues
    {
        Individual = 1,
        Minefield = 2
    }

    public class QuizType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}