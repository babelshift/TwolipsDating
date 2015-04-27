using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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