using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class Review
    {
        public Review()
        {
            Violations = new List<ReviewViolation>();
        }

        public int Id { get; set; }

        [IndexAttribute("UX_AuthorAndTarget", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string AuthorUserId { get; set; }

        [IndexAttribute("UX_AuthorAndTarget", 2, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string TargetUserId { get; set; }

        public DateTime DateCreated { get; set; }
        public string Content { get; set; }
        public int RatingValue { get; set; }

        public virtual ApplicationUser AuthorUser { get; set; }
        public virtual ApplicationUser TargetUser { get; set; }
        public virtual ReviewRating Rating { get; set; }

        public virtual ICollection<ReviewViolation> Violations { get; set; }
    }
}