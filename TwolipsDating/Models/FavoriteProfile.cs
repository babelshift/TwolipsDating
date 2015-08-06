using System;

namespace TwolipsDating.Models
{
    public class FavoriteProfile
    {
        public string UserId { get; set; }
        public int ProfileId { get; set; }
        public DateTime DateFavorited { get; set; }

        public ApplicationUser User { get; set; }
        public Profile Profile { get; set; }
    }
}