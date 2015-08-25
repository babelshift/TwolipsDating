using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class EmailNotifications
    {
        public string ApplicationUserId { get; set; }
        public bool SendNewFollowerNotifications { get; set; }
        public bool SendTagNotifications { get; set; }
        public bool SendGiftNotifications { get; set; }
        public bool SendMessageNotifications { get; set; }
        public bool SendReviewNotifications { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}