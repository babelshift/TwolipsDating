using System;

namespace TwolipsDating.Models
{
    public class MilestoneAchievement
    {
        public int MilestoneId { get; set; }
        public string UserId { get; set; }
        public DateTime DateAchieved { get; set; }

        public virtual Milestone Milestone { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}