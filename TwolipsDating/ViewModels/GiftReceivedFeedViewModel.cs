using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class GiftReceivedFeedViewModel
    {
        public string SenderUserName { get; set; }
        public int SenderProfileId { get; set; }
        public string SenderProfileImagePath { get; set; }
        public string ReceiverUserName { get; set; }
        public int ReceiverProfileId { get; set; }
        public string ReceiverProfileImagePath { get; set; }
        public DateTime DateSent { get; set; }
        public string TimeAgo { get; set; }
        public string GiftImagePath { get; set; }
        public string GiftName { get; set; }
        public string GiftDescription { get; set; }
        public int GiftSentCount { get; set; }
    }
}