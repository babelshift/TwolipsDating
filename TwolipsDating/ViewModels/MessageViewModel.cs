using System.Collections.Generic;

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
        public bool IsCurrentUserEmailConfirmed { get; set; }
        public MessageViewMode MessageViewMode { get; set; }
        public IReadOnlyCollection<ReceivedMessageViewModel> ReceivedMessages { get; set; }
        public IReadOnlyCollection<SentMessageViewModel> SentMessages { get; set; }
    }
}