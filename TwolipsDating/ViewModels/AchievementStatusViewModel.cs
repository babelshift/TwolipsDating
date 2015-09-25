using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public enum AchievementStatusType
    {
        Incomplete,
        Complete
    }

    public class AchievementStatusViewModel
    {
        public int AchievedCount { get; set; }
        public int RequiredCount { get; set; }
        public int PercentComplete
        {
            get
            {
                double value = (double)AchievedCount / (double)RequiredCount;
                double percent = value * 100;
                return (int)Math.Round(percent);
            }
        }
        public AchievementStatusType AchievementStatus
        {
            get
            {
                return AchievedCount >= RequiredCount ? AchievementStatusType.Complete : AchievementStatusType.Incomplete;
            }
        }
        public string AchievementIconPath { get; set; }
    }
}