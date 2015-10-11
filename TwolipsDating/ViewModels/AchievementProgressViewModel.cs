using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class AchievementProgressViewModel : AchievementStatusViewModel
    {
        public int IncreaseAchievedCount { get; set; }

        public int PreviousAchievedCount
        {
            get
            {
                return AchievedCount - IncreaseAchievedCount;
            }
        }

        public int PreviousPercentComplete
        {
            get
            {
                double value = (double)PreviousAchievedCount / (double)RequiredCount;
                double percent = value * 100;
                return (int)Math.Round(percent);
            }
        }
    }
}