using System;

namespace TwolipsDating.Models
{
    public class IgnoredUser
    {
        public string SourceUserId { get; set; }
        public string TargetUserId { get; set; }
        public DateTime DateIgnored { get; set; }

        public ApplicationUser SourceUser { get; set; }
        public ApplicationUser TargetUser { get; set; }
    }
}