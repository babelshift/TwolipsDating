using System;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class ProfileVisitFeedViewModel
    {
        public DateTime DateOccurred { get; set; }
        public string TimeAgo { get { return DateOccurred.GetTimeAgo(); } }

        public string VisitorUserId { get;  set;}
        public string VisitorSEOName { get { return ProfileExtensions.ToSEOName(VisitorUserName); } }
        public string VisitorUserName { get; set; }
        public int VisitorProfileId { get; set; }
        public string VisitorProfileImagePath { get; set; }
    }
}