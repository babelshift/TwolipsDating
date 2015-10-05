using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class AnsweredMinefieldQuestion
    {
        public string UserId { get; set; }
        public int MinefieldQuestionId { get; set; }
        public int MinefieldAnswerId { get; set; }
        public DateTime DateAnswered { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual MinefieldQuestion Question { get; set; }
        public virtual MinefieldAnswer Answer { get; set; }
    }
}