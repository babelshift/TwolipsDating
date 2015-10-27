using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class GiftReceivedFeedViewModel
    {
        public string SenderUserName { get; set; }
        public int SenderProfileId { get; set; }
        public string SenderSEOName { get { return ProfileExtensions.GetProfileSEOName(SenderUserName); } }
        public string SenderProfileImagePath { get; set; }
        public string SenderUserId { get; set; }
        public string ReceiverUserName { get; set; }
        public int ReceiverProfileId { get; set; }
        public string ReceiverSEOName { get { return ProfileExtensions.GetProfileSEOName(ReceiverUserName); } }
        public string ReceiverProfileImagePath { get; set; }
        public DateTime DateSent { get; set; }
        public string TimeAgo { get { return DateSent.GetTimeAgo(); } }
        public IDictionary<int, GiftReceivedFeedItemViewModel> Gifts { get; set; }

        public int StoreItemId { get; set; }

        public string StoreItemIconPath { get; set; }

        public int ItemCount { get; set; }
    }
}