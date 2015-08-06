using System;

namespace TwolipsDating.Models
{
    public class UserImage
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string FileName { get; set; }
        public DateTime DateUploaded { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}