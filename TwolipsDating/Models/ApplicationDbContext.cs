using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ZipCode> ZipCodes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewRating> ReviewRatings { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<TagSuggestion> TagSuggestions { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            SetupApplicationUserEntity(modelBuilder);
            SetupProfileEntity(modelBuilder);
            SetupGenderEntity(modelBuilder);
            SetupCountryEntity(modelBuilder);
            SetupCityEntity(modelBuilder);
            SetupUSStateEntity(modelBuilder);
            SetupZipCodeEntity(modelBuilder);
            SetupMessageStatusEntity(modelBuilder);
            SetupMessageEntity(modelBuilder);
            SetupReviewEntity(modelBuilder);
            SetupReviewRatingEntity(modelBuilder);
            SetupUserImageEntity(modelBuilder);
			SetupTagEntity(modelBuilder);
			SetupTagSuggestionEntity(modelBuilder);
        }

		private void SetupTagSuggestionEntity(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TagSuggestion>()
				.HasKey(t => new { t.TagId, t.ProfileId, t.SuggestingUserId });

			modelBuilder.Entity<TagSuggestion>()
				.Property(t => t.DateSuggested)
				.IsRequired();

			modelBuilder.Entity<TagSuggestion>()
				.HasRequired(t => t.Profile)
				.WithMany(t => t.TagSuggestions)
				.HasForeignKey(t => t.ProfileId);

			modelBuilder.Entity<TagSuggestion>()
				.HasRequired(t => t.SuggestingUser)
				.WithMany(t => t.TagSuggestions)
				.HasForeignKey(t => t.SuggestingUserId);

			modelBuilder.Entity<TagSuggestion>()
				.HasRequired(t => t.Tag)
				.WithMany(t => t.TagSuggestions)
				.HasForeignKey(t => t.TagId);
		}

		private void SetupTagEntity(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tag>().HasKey(t => t.TagId);

			modelBuilder.Entity<Tag>()
				.Property(g => g.TagId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			modelBuilder.Entity<Tag>()
				.Property(t => t.Name)
				.HasMaxLength(255);

			modelBuilder.Entity<Tag>()
				.Property(t => t.Name)
				.IsRequired();

			modelBuilder.Entity<Tag>()
				.HasMany(t => t.Profiles)
				.WithMany(t => t.Tags);
		}

        private void SetupUserImageEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserImage>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<UserImage>()
                .HasRequired(i => i.ApplicationUser)
                .WithMany(i => i.UploadedImages)
                .HasForeignKey(i => i.ApplicationUserId);

            modelBuilder.Entity<UserImage>()
                .Property(i => i.FileName)
                .IsRequired();

            modelBuilder.Entity<UserImage>()
                .Property(i => i.FileName)
                .HasMaxLength(64);

            modelBuilder.Entity<UserImage>()
                .Property(i => i.DateUploaded)
                .IsRequired();
        }

        private void SetupReviewRatingEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReviewRating>()
                .HasKey(r => r.Value);

            modelBuilder.Entity<ReviewRating>()
                .Property(r => r.Value)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        private void SetupReviewEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Review>()
                .HasRequired(m => m.AuthorUser)
                .WithMany(m => m.SentReviews)
                .HasForeignKey(m => m.AuthorUserId);

            modelBuilder.Entity<Review>()
                .HasRequired(m => m.TargetUser)
                .WithMany(m => m.ReceivedReviews)
                .HasForeignKey(m => m.TargetUserId);

            modelBuilder.Entity<Review>()
                .Property(m => m.DateCreated)
                .IsRequired();

            modelBuilder.Entity<Review>()
                .Property(m => m.Content)
                .IsRequired();

            modelBuilder.Entity<Review>()
                .Property(m => m.Content)
                .HasMaxLength(2000);

            modelBuilder.Entity<Review>()
                .HasRequired(m => m.Rating)
                .WithMany(m => m.Reviews)
                .HasForeignKey(m => m.RatingValue);
        }

        private void SetupMessageEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.SenderApplicationUser)
                .WithMany(m => m.SentMessages)
                .HasForeignKey(m => m.SenderApplicationUserId);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.ReceiverApplicationUser)
                .WithMany(m => m.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverApplicationUserId);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.MessageStatus)
                .WithMany(m => m.Messages)
                .HasForeignKey(m => m.MessageStatusId);

            modelBuilder.Entity<Message>()
                .Property(m => m.DateSent)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(m => m.Body)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(m => m.Body)
                .HasMaxLength(2000);
        }

        private void SetupMessageStatusEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageStatus>()
                .HasKey(ms => ms.Id);

            modelBuilder.Entity<MessageStatus>()
                .Property(ms => ms.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<MessageStatus>()
                .Property(ms => ms.Name)
                .IsRequired();

            modelBuilder.Entity<MessageStatus>()
                .Property(ms => ms.Name)
                .HasMaxLength(50);
        }

        private void SetupZipCodeEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ZipCode>()
                .HasKey(z => z.ZipCodeId);

            modelBuilder.Entity<ZipCode>()
                .HasRequired(z => z.City)
                .WithMany(z => z.ZipCodes)
                .HasForeignKey(z => z.CityId);

            modelBuilder.Entity<ZipCode>()
                .Property(z => z.ZipCodeId)
                .HasMaxLength(5);
        }

        private void SetupUSStateEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<USState>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<USState>()
                .Property(u => u.Abbreviation)
                .IsRequired();

            modelBuilder.Entity<USState>()
                .Property(u => u.Name)
                .IsRequired();

            modelBuilder.Entity<USState>()
                .Property(u => u.Abbreviation)
                .HasMaxLength(2);

            modelBuilder.Entity<USState>()
                .Property(u => u.Name)
                .HasMaxLength(50);
        }

        private void SetupApplicationUserEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(a => a.Profile)
                .WithRequired(e => e.ApplicationUser);

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.IsActive)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.DateCreated)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.DateLastLogin)
                .IsRequired();
        }

        private void SetupCityEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<City>()
                .HasRequired(c => c.Country)
                .WithMany(c => c.Cities)
                .HasForeignKey(c => c.CountryId);

            modelBuilder.Entity<City>()
                .HasOptional(c => c.USState)
                .WithMany(c => c.Cities)
                .HasForeignKey(c => c.USStateId);

            modelBuilder.Entity<City>()
                .Property(c => c.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<City>()
                .Property(c => c.Name)
                .IsRequired();
        }

        private void SetupCountryEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Country>()
                .Property(c => c.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Country>()
                .Property(c => c.Name)
                .IsRequired();
        }

        private void SetupGenderEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gender>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Gender>()
                .Property(g => g.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Gender>()
                .Property(g => g.Name)
                .HasMaxLength(25);

            modelBuilder.Entity<Gender>()
                .Property(g => g.Name)
                .IsRequired();
        }

        private void SetupProfileEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                .HasRequired(a => a.ApplicationUser)
                .WithOptional(a => a.Profile);

            modelBuilder.Entity<Profile>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Profile>()
                .HasRequired(p => p.Gender)
                .WithMany(p => p.Profiles)
                .HasForeignKey(p => p.GenderId);

            modelBuilder.Entity<Profile>()
                .HasRequired(p => p.City)
                .WithMany(p => p.Profiles)
                .HasForeignKey(p => p.CityId);

            modelBuilder.Entity<Profile>()
                .Property(p => p.Birthday)
                .IsRequired();

            modelBuilder.Entity<Profile>()
                .HasOptional(i => i.UserImage)
                .WithOptionalPrincipal(i => i.Profile);
        }
    }
}