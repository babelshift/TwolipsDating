using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class AchievementShowcaseItemViewModel
    {
        public int? MilestoneId { get; set; }
        public string AchievementImagePath { get; set; }
        public string AchievementName { get; set; }
        public string ProfileUserId { get; set; }
    }
}