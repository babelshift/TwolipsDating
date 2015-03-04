using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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