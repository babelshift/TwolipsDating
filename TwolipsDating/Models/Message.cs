using System;

namespace TwolipsDating.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderApplicationUserId { get; set; }
        public string ReceiverApplicationUserId { get; set; }
        public DateTime DateSent { get; set; }
        public string Body { get; set; }
        public int MessageStatusId { get; set; }

        public virtual MessageStatus MessageStatus { get; set; }
        public virtual ApplicationUser SenderApplicationUser { get; set; }
        public virtual ApplicationUser ReceiverApplicationUser { get; set; }
    }
}