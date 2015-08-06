using System;
using System.Collections.Generic;
using System.Linq;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class ReviewExtensions
    {
        public static int AverageRating<T>(this IReadOnlyCollection<T> reviews)
            where T : Review
        {
            if (reviews == null)
            {
                return 0;
            }

            if (reviews.Count <= 0)
            {
                return 0;
            }

            return (int)Math.Round(reviews.Average(r => r.RatingValue));
        }

        public static int AverageRating<T>(this ICollection<T> reviews)
            where T : Review
        {
            if (reviews == null)
            {
                return 0;
            }

            if (reviews.Count <= 0)
            {
                return 0;
            }

            return (int)Math.Round(reviews.Average(r => r.RatingValue));
        }
    }
}