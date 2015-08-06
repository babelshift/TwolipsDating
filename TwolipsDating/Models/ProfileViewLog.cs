using System;

namespace TwolipsDating.Models
{
    public class ProfileViewLog
    {
        public string ViewerUserId { get; set; }
        public int TargetProfileId { get; set; }
        public DateTime DateVisited { get; set; }

        public virtual ApplicationUser ViewerUser { get; set; }
        public virtual Profile Profile { get; set; }
    }
}