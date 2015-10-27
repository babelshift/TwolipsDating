using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class FollowerFeedViewModel
    {
        public int FollowerProfileId { get; set; }
        public string FollowerSEOName { get { return ProfileExtensions.GetProfileSEOName(FollowerName); } }
        public string FollowerProfileImagePath { get; set; }
        public string FollowerName { get; set; }
        public DateTime DateFollowed { get; set; }
        public string TimeAgo { get { return DateFollowed.GetTimeAgo(); } }
    }
}
