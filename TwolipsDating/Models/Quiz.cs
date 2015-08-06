using System;
using System.Collections.Generic;

namespace TwolipsDating.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<CompletedQuiz> CompletedByUsers { get; set; }
    }
}