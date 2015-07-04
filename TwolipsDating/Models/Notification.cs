using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int NotificationTypeId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual NotificationType NotificationType { get; set; }
    }
}