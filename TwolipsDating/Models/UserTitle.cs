using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class UserTitle
    {
        public string UserId { get; set; }
        public int TitleId { get; set; }
        public DateTime DateObtained { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Title Title { get; set; }
    }
}