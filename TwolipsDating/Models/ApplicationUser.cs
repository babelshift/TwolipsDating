using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace TwolipsDating.Models
{
    public class ApplicationUser : IdentityUser
    {
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateLastLogin { get; set; }

        public virtual Profile Profile { get; set; }

		public virtual ICollection<Review> SentReviews { get; set; }
		public virtual ICollection<Review> ReceivedReviews { get; set; }
		public virtual ICollection<Message> SentMessages { get; set; }
		public virtual ICollection<Message> ReceivedMessages { get; set; }
		public virtual ICollection<UserImage> UploadedImages { get; set; }
		public virtual ICollection<TagSuggestion> TagSuggestions { get; set; }
        public virtual ICollection<ReviewViolation> ReviewViolationsAuthored { get; set; }
        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
        public virtual ICollection<GiftTransactionLog> ItemsSent { get; set; }
        public virtual ICollection<GiftTransactionLog> ItemsReceived { get; set; }
        public virtual ICollection<FavoriteProfile> FavoriteProfiles { get; set; }
        public virtual ICollection<IgnoredUser> IgnoredUsers { get; set; }
        public virtual ICollection<IgnoredUser> IgnoredBy { get; set; }
        public virtual ICollection<AnsweredQuestion> AnsweredQuestions { get; set; }
        public virtual ICollection<MilestoneAchievement> MilestonesAchieved { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}