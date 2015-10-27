using System;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class MessageFeedViewModel
    {
        public DateTime DateOccurred { get; set; }
        public string SenderUserId { get; set; }
        public string SenderSEOName { get { return ProfileExtensions.ToSEOName(SenderUserName); } }
        public string SenderUserName { get; set; }
        public int SenderProfileId { get; set; }
        public string SenderProfileImagePath { get; set; }
        public string ReceiverUserId { get; set; }
        public string ReceiverUserName { get; set; }
        public int ReceiverProfileId { get; set; }
        public string ReceiverProfileImagePath { get; set; }
        public string TimeAgo { get { return DateOccurred.GetTimeAgo(); } }
        public string MessageContent { get; set; }
    }
}