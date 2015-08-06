using System.Collections.Generic;

namespace TwolipsDating.Models
{
    public class ReviewRating
    {
        public ReviewRating()
        {
            Reviews = new List<Review>();
        }

        public int Value { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}