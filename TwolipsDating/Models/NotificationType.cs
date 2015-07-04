using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public enum NotificationTypeValues
    {
        Message = 1,
        Gift = 2,
        Achievement = 3,
        Announcement = 4
    }

    public class NotificationType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}