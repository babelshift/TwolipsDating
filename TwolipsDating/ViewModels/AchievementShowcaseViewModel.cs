using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class AchievementShowcaseViewModel
    {
        public string ProfileUserId { get; set; }
        public IReadOnlyCollection<AchievementShowcaseItemViewModel> Items { get; set; }
    }
}