using System;

namespace TwolipsDating.Models
{
    public class ReviewViolation
    {
        public int Id { get; set; }
        public string AuthorUserId { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public int ReviewId { get; set; }
        public int ViolationTypeId { get; set; }

        public virtual ApplicationUser AuthorUser { get; set; }
        public virtual Review Review { get; set; }
        public virtual ViolationType ViolationType { get; set; }
    }
}