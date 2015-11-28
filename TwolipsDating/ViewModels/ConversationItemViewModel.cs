using System;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class ConversationItemViewModel
    {
        //public string SenderUserId { get; set; }
        //public int SenderProfileId { get; set; }
        //public string SenderName { get; set; }
        //public string SenderProfileImagePath { get; set; }

        public string TargetUserId { get; set; }
        public int TargetProfileId { get; set; }
        public string TargetName { get; set; }
        public string TargetProfileImagePath { get; set; }

        public DateTime DateSent { get; set; }
        public string TimeAgo { get { return DateSent.GetTimeAgo(); } }
        public string MostRecentMessageSenderUserId { get; set; }
        public string MostRecentMessageBody { get; set; }
        public int MostRecentMessageStatusId { get; set; }
    }
}