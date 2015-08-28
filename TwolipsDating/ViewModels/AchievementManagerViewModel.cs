using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class AchievementManagerViewModel
    {
        public IReadOnlyCollection<AchievementOverviewViewModel> Achievements { get; set; }
    }
}