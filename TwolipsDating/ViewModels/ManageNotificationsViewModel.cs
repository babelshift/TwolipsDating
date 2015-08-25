using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ManageNotificationsViewModel
    {
        public bool SendNewFollowerNotifications { get; set; }
        public bool SendTagNotifications { get; set; }
        public bool SendGiftNotifications { get; set; }
        public bool SendMessageNotifications { get; set; }
    }
}