using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class MessageConversation
    {
        public int MessageId { get; set; }
        public string SenderApplicationUserId { get; set; }
        public string SenderName { get; set; }
        public int SenderProfileId { get; set; }
        public string SenderProfileImageFileName { get; set; }
        public string ReceiverApplicationUserId { get; set; }
        public string ReceiverName { get; set; }
        public int ReceiverProfileId { get; set; }
        public string ReceiverProfileImageFileName { get; set; }
        public DateTime DateSent { get; set; }
        public string Body { get; set; }
        public int MessageStatusId { get; set; }
    }
}