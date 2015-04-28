using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public enum QuestionTypeValues
    {
        Random = 1,
        Timed = 2
    }

    public class QuestionType
    {
        public int Id { get; set; }

        [Index("UX_Name", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}