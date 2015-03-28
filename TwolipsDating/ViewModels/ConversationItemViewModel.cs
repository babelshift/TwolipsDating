using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ConversationItemViewModel
    {
        public string TargetUserId { get; set; }
        public int TargetProfileId { get; set; }
        public string TargetName { get; set; }
        public string TargetProfileImagePath { get; set; }
        public DateTime DateSent { get; set; }
        public string TimeAgo { get; set; }
        public int MostRecentMessageUserId { get; set; }
        public string MostRecentMessageBody { get; set; }
    }
}