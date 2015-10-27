using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class AchievementFeedViewModel
    {
        public int ProfileId { get; set; }
        public string ProfileSEOName
        {
            get
            {
                return ProfileExtensions.ToSEOName(UserName);
            }
        }
        public string UserName { get; set; }
        public string UserProfileImagePath { get; set; }
        public DateTime DateAchieved { get; set; }
        public string TimeAgo { get { return DateAchieved.GetTimeAgo(); } }
        public string AchievementName
        {
            get
            {
                return MilestoneAmountRequired > 1 ? String.Format("{0} ({1})", MilestoneTypeName, MilestoneAmountRequired) : MilestoneTypeName;
            }
        }
        public string AchievementIconPath { get; set; }
        public string MilestoneTypeName { get; set; }
        public int MilestoneAmountRequired { get; set; }
    }
}