using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public enum QuizCategoryValues
    {
        Uncategorized = 0,
        VideoGames = 1,
        Science = 2,
        History = 3,
        Business = 4,
        Food = 5,
        Technical = 6,
        Movies = 7,
        Creative = 8,
        Literature = 9,
        Television = 10,
        Sports = 11,
        Geography = 12,
        Math
    }

    public class QuizCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FontAwesomeIconName { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}