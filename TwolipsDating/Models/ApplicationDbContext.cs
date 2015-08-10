using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

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
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewRating> ReviewRatings { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagSuggestion> TagSuggestions { get; set; }
        public DbSet<MessageConversation> MessageConversations { get; set; }
        public DbSet<ViolationType> ViolationTypes { get; set; }
        public DbSet<ReviewViolation> ReviewViolations { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<GiftTransactionLog> GiftTransactions { get; set; }
        public DbSet<FavoriteProfile> FavoriteProfiles { get; set; }
        public DbSet<IgnoredUser> IgnoredUsers { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnsweredQuestion> AnsweredQuestions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<TagAward> TagAwards { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<MilestoneAchievement> MilestoneAchievements { get; set; }
        public DbSet<MilestoneType> MilestoneTypes { get; set; }
        public DbSet<CompletedQuiz> CompletedQuizzes { get; set; }
        public DbSet<StoreTransactionLog> StoreTransactions { get; set; }
        public DbSet<UserTitle> UserTitles { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<GeoCity> GeoCities { get; set; }
        public DbSet<GeoState> GeoStates { get; set; }
        public DbSet<GeoCountry> GeoCountries { get; set; }
        public DbSet<ProfileViewLog> ProfileViews { get; set; }
        public DbSet<QuestionViolationType> QuestionViolationTypes { get; set; }
        public DbSet<QuestionViolation> QuestionViolations { get; set; }
        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<StoreItemType> StoreItemTypes { get; set; }
        public DbSet<StoreSale> StoreSales { get; set; }
        public DbSet<StoreSpotlight> StoreSpotlights { get; set; }
        public DbSet<StoreGiftSpotlight> StoreGiftSpotlights { get; set; }
        public DbSet<TagAndSuggestedCount> TagsAndSuggestedCounts { get; set; }

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
            SetupMessageStatusEntity(modelBuilder);
            SetupMessageEntity(modelBuilder);
            SetupReviewEntity(modelBuilder);
            SetupReviewRatingEntity(modelBuilder);
            SetupUserImageEntity(modelBuilder);
            SetupTagEntity(modelBuilder);
            SetupTagSuggestionEntity(modelBuilder);
            SetupMessageConversations(modelBuilder);
            SetupViolationTypes(modelBuilder);
            SetupReviewViolations(modelBuilder);
            SetupInventoryItems(modelBuilder);
            SetupGiftTransactions(modelBuilder);
            SetupFavoriteProfiles(modelBuilder);
            SetupIgnoredUsers(modelBuilder);
            SetupQuizzes(modelBuilder);
            SetupQuestions(modelBuilder);
            SetupAnswers(modelBuilder);
            SetupAnsweredQuestions(modelBuilder);
            SetupQuestionTypes(modelBuilder);
            SetupTagAwards(modelBuilder);
            SetupMilestones(modelBuilder);
            SetupMilestoneAchievements(modelBuilder);
            SetupMilestoneTypes(modelBuilder);
            SetupCompletedQuizzes(modelBuilder);
            SetupStoreTransactions(modelBuilder);
            SetupUserTitles(modelBuilder);
            SetupAnnouncements(modelBuilder);
            SetupGeoCities(modelBuilder);
            SetupGeoStates(modelBuilder);
            SetupGeoCountries(modelBuilder);
            SetupProfileViews(modelBuilder);
            SetupQuestionViolationTypes(modelBuilder);
            SetupQuestionViolations(modelBuilder);
            SetupStoreItems(modelBuilder);
            SetupStoreItemTypes(modelBuilder);
            SetupStoreSales(modelBuilder);
            SetupStoreSpotlights(modelBuilder);
            SetupStoreGiftSpotlights(modelBuilder);
            SetupTagsAndSuggestedCountsView(modelBuilder);
        }

        private void SetupTagsAndSuggestedCountsView(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TagAndSuggestedCount>()
                .HasKey(m => new { m.Name });

            modelBuilder.Entity<TagAndSuggestedCount>()
                .ToTable("dbo.TagsAndSuggestedCountsView");
        }

        private void SetupStoreGiftSpotlights(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreGiftSpotlight>()
                .HasKey(v => new { v.StoreSaleId, v.DateStart, v.DateEnd });

            modelBuilder.Entity<StoreGiftSpotlight>()
                .HasRequired(v => v.StoreSale)
                .WithMany(v => v.StoreGiftSpotlights)
                .HasForeignKey(v => v.StoreSaleId);
        }

        private void SetupStoreSpotlights(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreSpotlight>()
                .HasKey(v => new { v.StoreSaleId, v.DateStart, v.DateEnd });

            modelBuilder.Entity<StoreSpotlight>()
                .HasRequired(v => v.StoreSale)
                .WithMany(v => v.StoreSpotlights)
                .HasForeignKey(v => v.StoreSaleId);
        }

        private void SetupStoreSales(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreSale>()
                .HasKey(v => v.SaleId);

            modelBuilder.Entity<StoreSale>()
                .HasRequired(v => v.StoreItem)
                .WithMany(v => v.StoreSales)
                .HasForeignKey(v => v.StoreItemId);
        }

        private void SetupStoreItemTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreItemType>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<StoreItemType>()
                .Property(v => v.Name)
                .HasMaxLength(64);

            modelBuilder.Entity<StoreItemType>()
                .Property(v => v.Name)
                .IsRequired();

            modelBuilder.Entity<StoreItemType>()
                .Property(v => v.Description)
                .HasMaxLength(255);

            modelBuilder.Entity<StoreItemType>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        private void SetupStoreItems(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreItem>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.Name)
                .HasMaxLength(64);

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.Name)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.Description)
                .HasMaxLength(255);

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.Description)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.IconFileName)
                .HasMaxLength(64);

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.IconFileName)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .HasRequired(v => v.ItemType)
                .WithMany(v => v.StoreItems)
                .HasForeignKey(v => v.ItemTypeId);

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<StoreItem>()
                .Property(v => v.DateAdded)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }

        private void SetupQuestionViolations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionViolation>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<QuestionViolation>()
                .HasRequired(p => p.Question)
                .WithMany(p => p.QuestionViolations)
                .HasForeignKey(p => p.QuestionId);

            modelBuilder.Entity<QuestionViolation>()
                .HasRequired(p => p.AuthorUser)
                .WithMany(p => p.QuestionViolationsAuthored)
                .HasForeignKey(p => p.AuthorUserId);

            modelBuilder.Entity<QuestionViolation>()
                .HasRequired(p => p.QuestionViolationType)
                .WithMany(p => p.QuestionViolations)
                .HasForeignKey(p => p.QuestionViolationTypeId);

            modelBuilder.Entity<QuestionViolation>().Property(p => p.AuthorUserId).IsRequired();
            modelBuilder.Entity<QuestionViolation>().Property(p => p.DateCreated).IsRequired();
        }

        private void SetupQuestionViolationTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionViolationType>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<QuestionViolationType>()
                .Property(v => v.Name)
                .HasMaxLength(64);

            modelBuilder.Entity<QuestionViolationType>()
                .Property(v => v.Name)
                .IsRequired();

            modelBuilder.Entity<QuestionViolationType>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        private void SetupProfileViews(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfileViewLog>()
                .HasKey(c => new { c.ViewerUserId, c.TargetProfileId, c.DateVisited });

            modelBuilder.Entity<ProfileViewLog>()
                .HasRequired(c => c.Profile)
                .WithMany(c => c.VisitedBy)
                .HasForeignKey(c => c.TargetProfileId);

            modelBuilder.Entity<ProfileViewLog>()
                .HasRequired(c => c.ViewerUser)
                .WithMany(c => c.ProfilesVisited)
                .HasForeignKey(c => c.ViewerUserId);
        }

        private void SetupGeoCountries(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoCountry>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<GeoCountry>()
                .Property(c => c.Name)
                .IsRequired();

            modelBuilder.Entity<GeoCountry>()
                .Property(c => c.Name)
                .HasMaxLength(255);
        }

        private void SetupGeoStates(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoState>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<GeoState>()
                .Property(c => c.Abbreviation)
                .IsRequired();

            modelBuilder.Entity<GeoState>()
                .Property(c => c.Abbreviation)
                .HasMaxLength(5);

            modelBuilder.Entity<GeoState>()
                .HasRequired(c => c.GeoCountry)
                .WithMany(c => c.GeoStates)
                .HasForeignKey(c => c.GeoCountryId);
        }

        private void SetupGeoCities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoCity>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<GeoCity>()
                .Property(c => c.Name)
                .IsRequired();

            modelBuilder.Entity<GeoCity>()
                .Property(c => c.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<GeoCity>()
                .HasRequired(c => c.GeoState)
                .WithMany(c => c.GeoCities)
                .HasForeignKey(c => c.GeoStateId);
        }

        private void SetupAnnouncements(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Announcement>()
                .Property(a => a.Content)
                .IsRequired();

            modelBuilder.Entity<Announcement>()
                .Property(a => a.Content)
                .HasMaxLength(1000);
        }

        private void SetupUserTitles(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTitle>()
                .HasKey(t => new { t.UserId, StoreItemId = t.StoreItemId });

            modelBuilder.Entity<UserTitle>()
                .Property(v => v.UserId)
                .IsRequired();

            modelBuilder.Entity<UserTitle>()
                .Property(v => v.DateObtained)
                .IsRequired();

            modelBuilder.Entity<UserTitle>()
                .HasRequired(v => v.StoreItem)
                .WithMany(v => v.OwnerUsers)
                .HasForeignKey(v => v.StoreItemId);

            modelBuilder.Entity<UserTitle>()
                .HasRequired(v => v.User)
                .WithMany(v => v.ObtainedTitles)
                .HasForeignKey(v => v.UserId);
        }

        private void SetupStoreTransactions(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreTransactionLog>()
                .HasKey(v => v.StoreTransactionLogId);

            modelBuilder.Entity<StoreTransactionLog>()
                .Property(v => v.ItemCount)
                .IsRequired();

            modelBuilder.Entity<StoreTransactionLog>()
                .HasRequired(p => p.BuyerUser)
                .WithMany(p => p.StoreTransactions)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<StoreTransactionLog>()
                .HasRequired(p => p.StoreItem)
                .WithMany(p => p.StoreTransactions)
                .HasForeignKey(p => p.StoreItemId);
        }

        //private void SetupTitles(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Title>()
        //        .HasKey(v => v.Id);

        //    modelBuilder.Entity<Title>()
        //        .Property(v => v.Name)
        //        .HasMaxLength(64);

        //    modelBuilder.Entity<Title>()
        //        .Property(v => v.Name)
        //        .IsRequired();

        //    modelBuilder.Entity<Title>()
        //        .Property(v => v.Description)
        //        .HasMaxLength(255);

        //    modelBuilder.Entity<Title>()
        //        .Property(v => v.Description)
        //        .IsRequired();

        //    modelBuilder.Entity<Title>()
        //        .Property(v => v.Id)
        //        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        //}

        private void SetupCompletedQuizzes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompletedQuiz>()
                .HasKey(t => new { t.UserId, t.QuizId });

            modelBuilder.Entity<CompletedQuiz>()
                .HasRequired(t => t.Quiz)
                .WithMany(t => t.CompletedByUsers)
                .HasForeignKey(t => t.QuizId);

            modelBuilder.Entity<CompletedQuiz>()
                .HasRequired(t => t.User)
                .WithMany(t => t.CompletedQuizzes)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<CompletedQuiz>()
                .Property(t => t.DateCompleted)
                .IsRequired();
        }

        private void SetupMilestones(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Milestone>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Milestone>()
                .HasRequired(t => t.MilestoneType)
                .WithMany(t => t.Milestones)
                .HasForeignKey(t => t.MilestoneTypeId);
        }

        private void SetupMilestoneAchievements(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MilestoneAchievement>()
                .HasKey(t => new { t.MilestoneId, t.UserId });

            modelBuilder.Entity<MilestoneAchievement>()
                .Property(t => t.DateAchieved)
                .IsRequired();

            modelBuilder.Entity<MilestoneAchievement>()
                .HasRequired(t => t.Milestone)
                .WithMany(t => t.MilestonesAchieved)
                .HasForeignKey(t => t.MilestoneId);

            modelBuilder.Entity<MilestoneAchievement>()
                .HasRequired(t => t.User)
                .WithMany(t => t.MilestonesAchieved)
                .HasForeignKey(t => t.UserId);
        }

        private void SetupMilestoneTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MilestoneType>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<MilestoneType>()
                .Property(v => v.Name)
                .HasMaxLength(64);

            modelBuilder.Entity<MilestoneType>()
                .Property(v => v.Name)
                .IsRequired();

            modelBuilder.Entity<MilestoneType>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        private void SetupTagAwards(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TagAward>()
                .HasKey(t => new { t.TagId, t.ProfileId, t.DateAwarded });

            modelBuilder.Entity<TagAward>()
                .HasRequired(t => t.Profile)
                .WithMany(t => t.TagAwards)
                .HasForeignKey(t => t.ProfileId);

            modelBuilder.Entity<TagAward>()
                .HasRequired(t => t.Tag)
                .WithMany(t => t.TagAwards)
                .HasForeignKey(t => t.TagId);
        }

        private void SetupQuestionTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionType>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<QuestionType>()
                .Property(v => v.Name)
                .HasMaxLength(64);

            modelBuilder.Entity<QuestionType>()
                .Property(v => v.Name)
                .IsRequired();

            modelBuilder.Entity<QuestionType>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        private void SetupAnsweredQuestions(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnsweredQuestion>()
                .HasKey(v => new { v.UserId, v.QuestionId });

            modelBuilder.Entity<AnsweredQuestion>()
                .Property(v => v.AnswerId)
                .IsRequired();

            modelBuilder.Entity<AnsweredQuestion>()
                .Property(v => v.DateAnswered)
                .IsRequired();

            modelBuilder.Entity<AnsweredQuestion>()
                .HasRequired(v => v.Answer)
                .WithMany(v => v.Instances)
                .HasForeignKey(v => v.AnswerId);

            modelBuilder.Entity<AnsweredQuestion>()
                .HasRequired(v => v.Question)
                .WithMany(v => v.AnsweredInstances)
                .HasForeignKey(v => v.QuestionId);

            modelBuilder.Entity<AnsweredQuestion>()
                .HasRequired(v => v.User)
                .WithMany(v => v.AnsweredQuestions)
                .HasForeignKey(v => v.UserId);
        }

        private void SetupAnswers(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>()
                .HasKey(v => new { v.Id });

            modelBuilder.Entity<Answer>()
                .Property(v => v.Content)
                .IsRequired();

            modelBuilder.Entity<Answer>()
                .Property(v => v.Content)
                .HasMaxLength(1000);

            modelBuilder.Entity<Answer>()
                .HasRequired(v => v.Question)
                .WithMany(v => v.PossibleAnswers)
                .HasForeignKey(v => v.QuestionId);
        }

        private void SetupQuestions(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>()
                .HasKey(v => new { v.Id });

            modelBuilder.Entity<Question>()
                .Property(v => v.Content)
                .IsRequired();

            modelBuilder.Entity<Question>()
                .Property(v => v.Content)
                .HasMaxLength(1000);

            modelBuilder.Entity<Question>()
                .HasOptional(v => v.QuestionType)
                .WithMany(v => v.Questions)
                .HasForeignKey(v => v.QuestionTypeId);

            modelBuilder.Entity<Question>()
                .Property(v => v.Points)
                .IsRequired();
        }

        private void SetupQuizzes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quiz>()
                .HasKey(v => new { v.Id });

            modelBuilder.Entity<Quiz>()
                .Property(v => v.Name)
                .IsRequired();

            modelBuilder.Entity<Quiz>()
                .Property(v => v.Description)
                .IsRequired();

            modelBuilder.Entity<Quiz>()
                .Property(v => v.DateCreated)
                .IsRequired();

            modelBuilder.Entity<Quiz>()
                .Property(v => v.Name)
                .HasMaxLength(64);

            modelBuilder.Entity<Quiz>()
                .Property(v => v.Description)
                .HasMaxLength(255);

            modelBuilder.Entity<Quiz>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        private void SetupIgnoredUsers(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgnoredUser>()
                .HasKey(v => new { v.SourceUserId, v.TargetUserId });

            modelBuilder.Entity<IgnoredUser>()
                .Property(v => v.DateIgnored)
                .IsRequired();

            modelBuilder.Entity<IgnoredUser>()
                .HasRequired(v => v.SourceUser)
                .WithMany(v => v.IgnoredUsers)
                .HasForeignKey(v => v.SourceUserId);

            modelBuilder.Entity<IgnoredUser>()
                .HasRequired(v => v.TargetUser)
                .WithMany(v => v.IgnoredBy)
                .HasForeignKey(v => v.TargetUserId);
        }

        private void SetupFavoriteProfiles(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavoriteProfile>()
                .HasKey(v => new { v.UserId, v.ProfileId });

            modelBuilder.Entity<FavoriteProfile>()
                .Property(v => v.DateFavorited)
                .IsRequired();

            modelBuilder.Entity<FavoriteProfile>()
                .HasRequired(v => v.User)
                .WithMany(v => v.FavoriteProfiles)
                .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<FavoriteProfile>()
                .HasRequired(v => v.Profile)
                .WithMany(v => v.FavoritedBy)
                .HasForeignKey(v => v.ProfileId);
        }

        private void SetupGiftTransactions(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GiftTransactionLog>()
                .HasKey(v => v.GiftTransactionLogId);

            modelBuilder.Entity<GiftTransactionLog>()
                .Property(v => v.FromUserId)
                .IsRequired();

            modelBuilder.Entity<GiftTransactionLog>()
                .Property(v => v.StoreItemId)
                .IsRequired();

            modelBuilder.Entity<GiftTransactionLog>()
                .Property(v => v.ItemCount)
                .IsRequired();

            modelBuilder.Entity<GiftTransactionLog>()
                .Property(v => v.ToUserId)
                .IsRequired();

            modelBuilder.Entity<GiftTransactionLog>()
                .HasRequired(p => p.FromUser)
                .WithMany(p => p.ItemsSent)
                .HasForeignKey(p => p.FromUserId);

            modelBuilder.Entity<GiftTransactionLog>()
                .HasRequired(p => p.ToUser)
                .WithMany(p => p.ItemsReceived)
                .HasForeignKey(p => p.ToUserId);

            modelBuilder.Entity<GiftTransactionLog>()
                .HasRequired(p => p.StoreItem)
                .WithMany(p => p.GiftTransactions)
                .HasForeignKey(p => p.StoreItemId);
        }

        private void SetupInventoryItems(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>()
                .HasKey(v => v.InventoryItemId);

            modelBuilder.Entity<InventoryItem>()
                .Property(v => v.ApplicationUserId)
                .IsRequired();

            modelBuilder.Entity<InventoryItem>()
                .Property(v => v.StoreItemId)
                .IsRequired();

            modelBuilder.Entity<InventoryItem>()
                .Property(v => v.ItemCount)
                .IsRequired();

            modelBuilder.Entity<InventoryItem>()
                .HasRequired(p => p.StoreItem)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(p => p.StoreItemId);

            modelBuilder.Entity<InventoryItem>()
                .HasRequired(p => p.OwnerUser)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(p => p.ApplicationUserId);
        }

        //private void SetupGifts(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Gift>()
        //        .HasKey(v => v.Id);

        //    modelBuilder.Entity<Gift>()
        //        .Property(v => v.Name)
        //        .HasMaxLength(64);

        //    modelBuilder.Entity<Gift>()
        //        .Property(v => v.Name)
        //        .IsRequired();

        //    modelBuilder.Entity<Gift>()
        //        .Property(v => v.Description)
        //        .HasMaxLength(255);

        //    modelBuilder.Entity<Gift>()
        //        .Property(v => v.Description)
        //        .IsRequired();

        //    modelBuilder.Entity<Gift>()
        //        .Property(v => v.IconFileName)
        //        .HasMaxLength(64);

        //    modelBuilder.Entity<Gift>()
        //        .Property(v => v.IconFileName)
        //        .IsRequired();

        //    modelBuilder.Entity<Gift>()
        //        .Property(v => v.Id)
        //        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        //}

        private void SetupReviewViolations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReviewViolation>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<ReviewViolation>()
                .HasRequired(p => p.Review)
                .WithMany(p => p.Violations)
                .HasForeignKey(p => p.ReviewId);

            modelBuilder.Entity<ReviewViolation>()
                .HasRequired(p => p.AuthorUser)
                .WithMany(p => p.ReviewViolationsAuthored)
                .HasForeignKey(p => p.AuthorUserId);

            modelBuilder.Entity<ReviewViolation>()
                .HasRequired(p => p.ViolationType)
                .WithMany(p => p.ReviewViolations)
                .HasForeignKey(p => p.ViolationTypeId);

            modelBuilder.Entity<ReviewViolation>().Property(r => r.Content).HasMaxLength(3000);

            modelBuilder.Entity<ReviewViolation>().Property(p => p.AuthorUserId).IsRequired();
            modelBuilder.Entity<ReviewViolation>().Property(p => p.Content).IsRequired();
            modelBuilder.Entity<ReviewViolation>().Property(p => p.DateCreated).IsRequired();
            modelBuilder.Entity<ReviewViolation>().Property(p => p.ReviewId).IsRequired();
        }

        private void SetupViolationTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViolationType>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<ViolationType>()
                .Property(v => v.Name)
                .HasMaxLength(64);

            modelBuilder.Entity<ViolationType>()
                .Property(v => v.Name)
                .IsRequired();

            modelBuilder.Entity<ViolationType>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        private void SetupMessageConversations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageConversation>()
                .HasKey(m => m.MessageId);

            modelBuilder.Entity<MessageConversation>()
                .ToTable("dbo.MessageConversations");
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

        //private void SetupZipCodeEntity(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ZipCode>()
        //        .HasKey(z => z.ZipCodeId);

        //    modelBuilder.Entity<ZipCode>()
        //        .HasRequired(z => z.City)
        //        .WithMany(z => z.ZipCodes)
        //        .HasForeignKey(z => z.CityId);

        //    modelBuilder.Entity<ZipCode>()
        //        .Property(z => z.ZipCodeId)
        //        .HasMaxLength(5);
        //}

        //private void SetupUSStateEntity(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<USState>()
        //        .HasKey(u => u.Id);

        //    modelBuilder.Entity<USState>()
        //        .Property(u => u.Abbreviation)
        //        .IsRequired();

        //    modelBuilder.Entity<USState>()
        //        .Property(u => u.Name)
        //        .IsRequired();

        //    modelBuilder.Entity<USState>()
        //        .Property(u => u.Abbreviation)
        //        .HasMaxLength(2);

        //    modelBuilder.Entity<USState>()
        //        .Property(u => u.Name)
        //        .HasMaxLength(50);
        //}

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

        //private void SetupCityEntity(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<City>()
        //        .HasKey(c => c.Id);

        //    modelBuilder.Entity<City>()
        //        .HasRequired(c => c.Country)
        //        .WithMany(c => c.Cities)
        //        .HasForeignKey(c => c.CountryId);

        //    modelBuilder.Entity<City>()
        //        .HasOptional(c => c.USState)
        //        .WithMany(c => c.Cities)
        //        .HasForeignKey(c => c.USStateId);

        //    modelBuilder.Entity<City>()
        //        .Property(c => c.Name)
        //        .HasMaxLength(255);

        //    modelBuilder.Entity<City>()
        //        .Property(c => c.Name)
        //        .IsRequired();
        //}

        //private void SetupCountryEntity(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Country>()
        //        .HasKey(c => c.Id);

        //    modelBuilder.Entity<Country>()
        //        .Property(c => c.Name)
        //        .HasMaxLength(255);

        //    modelBuilder.Entity<Country>()
        //        .Property(c => c.Name)
        //        .IsRequired();
        //}

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
                .HasOptional(p => p.SelectedTitle)
                .WithMany(p => p.SelectedByProfiles)
                .HasForeignKey(p => p.SelectedTitleId);

            modelBuilder.Entity<Profile>()
                .HasRequired(p => p.Gender)
                .WithMany(p => p.Profiles)
                .HasForeignKey(p => p.GenderId);

            modelBuilder.Entity<Profile>()
                .HasRequired(p => p.GeoCity)
                .WithMany(p => p.Profiles)
                .HasForeignKey(p => p.GeoCityId);

            modelBuilder.Entity<Profile>()
                .Property(p => p.Birthday)
                .IsRequired();
        }
    }
}