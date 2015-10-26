using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class AchievementFeedViewModel
    {
        public string ProfileId { get; set; }
        public string ProfileSEOName
        {
            get
            {
                return ProfileExtensions.GetProfileSEOName(UserName);
            }
        }
        public string UserName { get; set; }
        public string UserProfileImagePath { get; set; }
        public DateTime DateAchieved { get; set; }
        public string TimeAgo { get { return DateAchieved.GetTimeAgo(); } }
        public string AchievementName { get; set; }
        public string AchievementIconPath { get; set; }
    }
}