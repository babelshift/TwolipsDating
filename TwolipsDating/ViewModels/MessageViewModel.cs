using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public enum MessageViewMode
    {
        Conversation,
        Received,
        Sent
    }

    public class MessageViewModel
    {
        public MessageViewMode MessageViewMode { get; set; }
        public IReadOnlyCollection<ReceivedMessageViewModel> ReceivedMessages { get; set; }
        public IReadOnlyCollection<SentMessageViewModel> SentMessages { get; set; }
    }
}