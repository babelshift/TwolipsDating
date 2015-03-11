using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class MessageFeedViewModel
    {
        public DateTime DateOccurred { get; set; }
        public string SenderUserName { get; set; }
        public string SenderProfileId { get; set; }
        public string SenderProfileImagePath { get; set; }
        public string ReceiverProfileId { get; set; }
        public string ReceiverUserName { get; set; }
        public string ReceiverProfileImagePath { get; set; }
        public string TimeAgo { get; set; }
        public string MessageContent { get; set; }
    }
}