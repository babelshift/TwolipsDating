using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class AchievementOverviewViewModel
    {
        public string AchievementTypeName { get; set; }
        public string AchievementDescription { get; set; }
        public IReadOnlyCollection<AchievementStatusViewModel> AchievementStatuses { get; set; }
    }
}