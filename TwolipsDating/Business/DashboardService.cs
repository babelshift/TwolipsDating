using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public enum FeedItemQueryType
    {
        All,
        Self
    }

    public class DashboardService : BaseService, IDashboardService
    {
        #region Constructor

        public DashboardService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        internal static DashboardService Create(IdentityFactoryOptions<DashboardService> options, IOwinContext context)
        {
            var service = new DashboardService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

        #endregion Constructor

        #region Uploaded Images

        public async Task<IReadOnlyCollection<UploadedImageFeedViewModel>> GetUploadedImagesForUserFeedAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var imagesUploaded = (from userImages in db.UserImages
                                  where userImages.ApplicationUserId == userId
                                  where userImages.IsBanner == false
                                  select new UploadedImageFeedViewModel()
                                  {
                                      DateOccurred = userImages.DateUploaded,
                                      UploaderProfileId = userImages.ApplicationUser.Profile.Id,
                                      UploaderProfileImagePath = userImages.ApplicationUser.Profile.UserImage.FileName,
                                      UploaderUserId = userImages.ApplicationUserId,
                                      UploaderUserName = userImages.ApplicationUser.UserName,
                                      Path = userImages.FileName
                                  });

            var results = await imagesUploaded.ToListAsync();

            foreach (var image in results)
            {
                image.UploaderProfileImagePath = ProfileExtensions.GetThumbnailImagePath(image.UploaderProfileImagePath);
                image.UploadedImagesPaths = new List<UploadedImageViewModel>();
                image.UploadedImagesPaths.Add(new UploadedImageViewModel()
                {
                    Path = UserImageExtensions.GetPath(image.Path)
                });
            }

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns all uploaded images for profiles that the passed user id has marked as "favorite"
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<UploadedImageFeedViewModel>> GetFollowerUploadedImagesAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var imagesUploadedByFavorites = (from userImages in db.UserImages
                                             join favoritedProfiles in db.FavoriteProfiles on userImages.ApplicationUser.Profile.Id equals favoritedProfiles.ProfileId
                                             where favoritedProfiles.UserId == userId
                                             where userImages.ApplicationUser.IsActive
                                             where userImages.IsBanner == false
                                             select new UploadedImageFeedViewModel()
                                             {
                                                 DateOccurred = userImages.DateUploaded,
                                                 UploaderProfileId = userImages.ApplicationUser.Profile.Id,
                                                 UploaderProfileImagePath = userImages.ApplicationUser.Profile.UserImage.FileName,
                                                 UploaderUserId = userImages.ApplicationUserId,
                                                 UploaderUserName = userImages.ApplicationUser.UserName,
                                                 Path = userImages.FileName
                                             });

            var results = await imagesUploadedByFavorites.ToListAsync();

            foreach (var image in results)
            {
                image.UploaderProfileImagePath = ProfileExtensions.GetThumbnailImagePath(image.UploaderProfileImagePath);
                image.UploadedImagesPaths = new List<UploadedImageViewModel>();
                image.UploadedImagesPaths.Add(new UploadedImageViewModel()
                {
                    Path = UserImageExtensions.GetPath(image.Path)
                });
            }

            return results.AsReadOnly();
        }

        #endregion Uploaded Images

        #region Reviews

        /// <summary>
        /// Returns all reviews written by and for users marked as favorites for the passed user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<ReviewWrittenFeedViewModel>> GetReviewsForFeedAsync(string userId, FeedItemQueryType queryType)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            List<ReviewWrittenFeedViewModel> results = new List<ReviewWrittenFeedViewModel>();

            // get all reviews written by or for anyone that we are following
            if (queryType == FeedItemQueryType.All)
            {
                // get the reviews that were written for the favorited users
                var reviewsWritten = (from reviews in db.Reviews
                                      from favoritedProfiles in db.FavoriteProfiles
                                      where reviews.TargetUser.Profile.Id == favoritedProfiles.ProfileId
                                      || reviews.AuthorUser.Profile.Id == favoritedProfiles.ProfileId
                                      where favoritedProfiles.UserId == userId
                                      where reviews.TargetUser.IsActive
                                      where reviews.AuthorUser.IsActive
                                      select new ReviewWrittenFeedViewModel()
                                      {
                                          AuthorProfileId = reviews.AuthorUser.Profile.Id,
                                          AuthorProfileImagePath = reviews.AuthorUser.Profile.UserImage.FileName,
                                          AuthorUserName = reviews.AuthorUser.UserName,
                                          DateOccurred = reviews.DateCreated,
                                          ReviewContent = reviews.Content,
                                          ReviewId = reviews.Id,
                                          ReviewRatingValue = reviews.RatingValue,
                                          TargetProfileId = reviews.TargetUser.Profile.Id,
                                          TargetProfileImagePath = reviews.TargetUser.Profile.UserImage.FileName,
                                          TargetUserName = reviews.TargetUser.UserName
                                      })
                                      .Distinct();

                results = await reviewsWritten.ToListAsync();
            }
            // get only reviews that were written about our self
            else
            {
                var reviewsWrittenForSelf = from reviews in db.Reviews
                                            where reviews.TargetUserId == userId
                                            where reviews.TargetUser.IsActive
                                            where reviews.AuthorUser.IsActive
                                            select new ReviewWrittenFeedViewModel()
                                            {
                                                AuthorProfileId = reviews.AuthorUser.Profile.Id,
                                                AuthorProfileImagePath = reviews.AuthorUser.Profile.UserImage.FileName,
                                                AuthorUserName = reviews.AuthorUser.UserName,
                                                DateOccurred = reviews.DateCreated,
                                                ReviewContent = reviews.Content,
                                                ReviewId = reviews.Id,
                                                ReviewRatingValue = reviews.RatingValue,
                                                TargetProfileId = reviews.TargetUser.Profile.Id,
                                                TargetProfileImagePath = reviews.TargetUser.Profile.UserImage.FileName,
                                                TargetUserName = reviews.TargetUser.UserName
                                            };

                results = await reviewsWrittenForSelf.ToListAsync();
            }

            foreach (var result in results)
            {
                result.AuthorProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.AuthorProfileImagePath);
                result.TargetProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.TargetProfileImagePath);
            }

            return results.AsReadOnly();
        }

        #endregion Reviews

        #region Gifts

        public async Task<IReadOnlyCollection<GiftReceivedFeedViewModel>> GetGiftTransactionsForUserFeedAsync(string userId)
        {
            var giftTransactions = from gifts in db.GiftTransactions
                                   where (gifts.FromUserId == userId || gifts.ToUserId == userId)
                                   where gifts.FromUser.IsActive
                                   where gifts.ToUser.IsActive
                                   select new GiftReceivedFeedViewModel()
                                   {
                                       DateSent = gifts.DateTransactionOccurred,
                                       ReceiverUserName = gifts.ToUser.UserName,
                                       ReceiverProfileImagePath = gifts.ToUser.Profile.UserImage.FileName,
                                       ReceiverProfileId = gifts.ToUser.Profile.Id,
                                       SenderProfileId = gifts.FromUser.Profile.Id,
                                       SenderProfileImagePath = gifts.FromUser.Profile.UserImage.FileName,
                                       SenderUserId = gifts.FromUser.Id,
                                       SenderUserName = gifts.FromUser.UserName,
                                       StoreItemId = gifts.StoreItemId,
                                       ItemCount = gifts.ItemCount,
                                       StoreItemIconPath = gifts.StoreItem.IconFileName
                                   };

            var results = await giftTransactions.ToListAsync();

            foreach (var result in results)
            {
                result.ReceiverProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.ReceiverProfileImagePath);
                result.SenderProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.SenderProfileImagePath);
                result.StoreItemIconPath = StoreItemExtensions.GetImagePath(result.StoreItemIconPath);
                result.Gifts = new Dictionary<int, GiftReceivedFeedItemViewModel>();
            }

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<GiftReceivedFeedViewModel>> GetFollowerGiftTransactionsAsync(string userId, FeedItemQueryType queryType)
        {
            List<GiftReceivedFeedViewModel> results = new List<GiftReceivedFeedViewModel>();

            if (queryType == FeedItemQueryType.All)
            {
                var giftsSent = (from gifts in db.GiftTransactions
                                 from favoritedProfiles in db.FavoriteProfiles
                                 where gifts.FromUser.Profile.Id == favoritedProfiles.ProfileId
                                 || gifts.ToUser.Profile.Id == favoritedProfiles.ProfileId
                                 where favoritedProfiles.UserId == userId
                                 where gifts.FromUser.IsActive
                                 where gifts.ToUser.IsActive
                                 select new GiftReceivedFeedViewModel()
                                 {
                                     DateSent = gifts.DateTransactionOccurred,
                                     ReceiverUserName = gifts.ToUser.UserName,
                                     ReceiverProfileImagePath = gifts.ToUser.Profile.UserImage.FileName,
                                     ReceiverProfileId = gifts.ToUser.Profile.Id,
                                     SenderProfileId = gifts.FromUser.Profile.Id,
                                     SenderProfileImagePath = gifts.FromUser.Profile.UserImage.FileName,
                                     SenderUserId = gifts.FromUser.Id,
                                     SenderUserName = gifts.FromUser.UserName,
                                     StoreItemId = gifts.StoreItemId,
                                     ItemCount = gifts.ItemCount,
                                     StoreItemIconPath = gifts.StoreItem.IconFileName
                                 })
                                 .Distinct();

                results = await giftsSent.ToListAsync();
            }
            else
            {
                var giftsToSelf = from gifts in db.GiftTransactions
                                  .Include(g => g.StoreItem)
                                  .Include(g => g.FromUser)
                                  where gifts.ToUserId == userId
                                  where gifts.FromUser.IsActive
                                  where gifts.ToUser.IsActive
                                  select new GiftReceivedFeedViewModel()
                                  {
                                      DateSent = gifts.DateTransactionOccurred,
                                      ReceiverUserName = gifts.ToUser.UserName,
                                      ReceiverProfileImagePath = gifts.ToUser.Profile.UserImage.FileName,
                                      ReceiverProfileId = gifts.ToUser.Profile.Id,
                                      SenderProfileId = gifts.FromUser.Profile.Id,
                                      SenderProfileImagePath = gifts.FromUser.Profile.UserImage.FileName,
                                      SenderUserId = gifts.FromUser.Id,
                                      SenderUserName = gifts.FromUser.UserName,
                                      StoreItemId = gifts.StoreItemId,
                                      ItemCount = gifts.ItemCount,
                                      StoreItemIconPath = gifts.StoreItem.IconFileName
                                  };

                results = await giftsToSelf.ToListAsync();
            }

            foreach (var result in results)
            {
                result.ReceiverProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.ReceiverProfileImagePath);
                result.SenderProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.SenderProfileImagePath);
                result.StoreItemIconPath = StoreItemExtensions.GetImagePath(result.StoreItemIconPath);
                result.Gifts = new Dictionary<int, GiftReceivedFeedItemViewModel>();
            }

            return results.AsReadOnly();
        }

        #endregion Gifts

        #region Quiz Completions

        public async Task<IReadOnlyCollection<CompletedQuizFeedViewModel>> GetQuizCompletionsForUserFeedAsync(string userId)
        {
            string sql = @"
select
	p.Id SourceProfileId,
	u.UserName SourceUserName,
	ui.FileName SourceProfileImagePath,
	q.Name QuizName,
	q.Id QuizId,
    q.ImageFileName as QuizThumbnailImagePath,
	cq.DateCompleted DateCompleted,
	(
        case
            when q.QuizTypeId = @quizTypeIndividual
            then (
                select count(*)
		        from dbo.QuestionQuizs qq2
		        inner join dbo.AnsweredQuestions aq on aq.UserId = cq.UserId and aq.QuestionId = qq2.Question_Id
		        inner join dbo.Questions qu on qu.Id = aq.QuestionId
		        where qq2.Quiz_Id = q.Id
		        and aq.AnswerId = qu.CorrectAnswerId
            )
            else (
                select count(*)
                from dbo.answeredminefieldquestions amq
                inner join dbo.MinefieldAnswers ma on ma.Id = amq.MinefieldAnswerId
                inner join dbo.MinefieldQuestions mq on mq.MinefieldQuestionId = amq.MinefieldQuestionId
                where mq.MinefieldQuestionId = q.Id
                and ma.IsCorrect = 1
				and amq.UserId = u.id
            )
        end
	) CorrectAnswerCount,
	(
        case
            when q.QuizTypeId = @quizTypeIndividual
            then (
		        select count(*)
		        from dbo.QuestionQuizs qq2
		        where qq2.Quiz_Id = q.Id
            )
            else (
		        select count(*)
                from dbo.MinefieldAnswers ma
                where MinefieldQuestionId = q.Id
                and ma.IsCorrect = 1
            )
        end
	) TotalAnswerCount
from dbo.CompletedQuizs cq
inner join dbo.Quizs q on cq.QuizId = q.Id
inner join dbo.AspNetUsers u on cq.UserId = u.Id
inner join dbo.Profiles p on p.ApplicationUser_Id = u.Id
left join dbo.UserImages ui on p.UserImageId = ui.Id
where
	cq.UserId = @userId
	and u.IsActive = 1";

            var results = await QueryAsync<CompletedQuizFeedViewModel>(sql, new { userId = userId, quizTypeIndividual = (int)QuizTypeValues.Individual });

            foreach (var result in results)
            {
                result.SourceProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.SourceProfileImagePath);
                result.QuizThumbnailImagePath = QuizExtensions.GetThumbnailImagePath(result.QuizThumbnailImagePath);
            }

            return results.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<CompletedQuizFeedViewModel>> GetFollowerQuizCompletionsAsync(string userId)
        {
            string sql = @"
select
	p.Id SourceProfileId,
	u.UserName SourceUserName,
	ui.FileName SourceProfileImagePath,
	q.Name QuizName,
	q.Id QuizId,
    q.ImageFileName as QuizThumbnailImagePath,
	cq.DateCompleted DateCompleted,
	(
        case
            when q.QuizTypeId = @quizTypeIndividual
            then (
                select count(*)
		        from dbo.QuestionQuizs qq2
		        inner join dbo.AnsweredQuestions aq on aq.UserId = cq.UserId and aq.QuestionId = qq2.Question_Id
		        inner join dbo.Questions qu on qu.Id = aq.QuestionId
		        where qq2.Quiz_Id = q.Id
		        and aq.AnswerId = qu.CorrectAnswerId
            )
            else (
                select count(*)
                from dbo.answeredminefieldquestions amq
                inner join dbo.MinefieldAnswers ma on ma.Id = amq.MinefieldAnswerId
                inner join dbo.MinefieldQuestions mq on mq.MinefieldQuestionId = amq.MinefieldQuestionId
                where mq.MinefieldQuestionId = q.Id
                and ma.IsCorrect = 1
				and amq.UserId = u.id
            )
        end
	) CorrectAnswerCount,
	(
        case
            when q.QuizTypeId = @quizTypeIndividual
            then (
		        select count(*)
		        from dbo.QuestionQuizs qq2
		        where qq2.Quiz_Id = q.Id
            )
            else (
		        select count(*)
                from dbo.MinefieldAnswers ma
                where MinefieldQuestionId = q.Id
                and ma.IsCorrect = 1
            )
        end
	) TotalAnswerCount
from dbo.CompletedQuizs cq
inner join dbo.Quizs q on cq.QuizId = q.Id
inner join dbo.AspNetUsers u on cq.UserId = u.Id
inner join dbo.Profiles p on p.ApplicationUser_Id = u.Id
left join dbo.UserImages ui on p.UserImageId = ui.Id
inner join dbo.FavoriteProfiles fp on p.Id = fp.ProfileId
where
	fp.UserId = @userId
	and u.IsActive = 1";

            var results = await QueryAsync<CompletedQuizFeedViewModel>(sql, new { userId = userId, quizTypeIndividual = (int)QuizTypeValues.Individual });

            foreach (var result in results)
            {
                result.SourceProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.SourceProfileImagePath);
                result.QuizThumbnailImagePath = QuizExtensions.GetThumbnailImagePath(result.QuizThumbnailImagePath);
            }

            return results.ToList().AsReadOnly();
        }

        #endregion Quiz Completions

        #region Tag Suggestions

        public async Task<IReadOnlyCollection<TagSuggestionReceivedFeedViewModel>> GetFollowerTagSuggestionsForUserFeedAsync(string userId)
        {
            var tagSuggestionsForUser = from tagSuggestion in db.TagSuggestions
                                        where (tagSuggestion.SuggestingUserId == userId || tagSuggestion.Profile.ApplicationUser.Id == userId)
                                        where tagSuggestion.SuggestingUser.IsActive
                                        where tagSuggestion.Profile.ApplicationUser.IsActive
                                        select new TagSuggestionReceivedFeedViewModel()
                                        {
                                            DateSuggested = tagSuggestion.DateSuggested,
                                            ReceiverProfileId = tagSuggestion.ProfileId,
                                            ReceiverUserName = tagSuggestion.Profile.ApplicationUser.UserName,
                                            SuggestProfileId = tagSuggestion.SuggestingUser.Profile.Id,
                                            SuggestProfileImagePath = tagSuggestion.SuggestingUser.Profile.UserImage.FileName,
                                            SuggestUserId = tagSuggestion.SuggestingUserId,
                                            SuggestUserName = tagSuggestion.SuggestingUser.UserName,
                                            TagName = tagSuggestion.Tag.Name
                                        };

            var results = await tagSuggestionsForUser.ToListAsync();

            foreach (var result in results)
            {
                result.SuggestProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.SuggestProfileImagePath);
                result.Tags = new List<string>();
                result.Tags.Add(result.TagName);
            }

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<TagSuggestionReceivedFeedViewModel>> GetFollowerTagSuggestionsAsync(string userId)
        {
            var tagSuggestionsSent = (from tagSuggestion in db.TagSuggestions
                                      from favoritedProfiles in db.FavoriteProfiles
                                      where tagSuggestion.Profile.Id == favoritedProfiles.ProfileId
                                      || tagSuggestion.SuggestingUser.Profile.Id == favoritedProfiles.ProfileId
                                      where favoritedProfiles.UserId == userId
                                      where tagSuggestion.SuggestingUser.IsActive
                                      where tagSuggestion.Profile.ApplicationUser.IsActive
                                      select new TagSuggestionReceivedFeedViewModel()
                                      {
                                          DateSuggested = tagSuggestion.DateSuggested,
                                          ReceiverProfileId = tagSuggestion.ProfileId,
                                          ReceiverUserName = tagSuggestion.Profile.ApplicationUser.UserName,
                                          SuggestProfileId = tagSuggestion.SuggestingUser.Profile.Id,
                                          SuggestProfileImagePath = tagSuggestion.SuggestingUser.Profile.UserImage.FileName,
                                          SuggestUserId = tagSuggestion.SuggestingUserId,
                                          SuggestUserName = tagSuggestion.SuggestingUser.UserName,
                                          TagName = tagSuggestion.Tag.Name
                                      })
                                     .Distinct();

            var results = await tagSuggestionsSent.ToListAsync();

            foreach (var result in results)
            {
                result.SuggestProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.SuggestProfileImagePath);
                result.Tags = new List<string>();
                result.Tags.Add(result.TagName);
            }

            return results.AsReadOnly();
        }

        #endregion Tag Suggestions

        #region Achievements

        public async Task<IReadOnlyCollection<AchievementFeedViewModel>> GetFollowerAchievementsForUserFeedAsync(string userId)
        {
            var achievements = from achievement in db.MilestoneAchievements
                               where achievement.UserId == userId
                               where achievement.User.IsActive
                               select new AchievementFeedViewModel()
                               {
                                   AchievementIconPath = achievement.Milestone.IconFileName,
                                   MilestoneAmountRequired = achievement.Milestone.AmountRequired,
                                   MilestoneTypeName = achievement.Milestone.MilestoneType.Name,
                                   DateAchieved = achievement.DateAchieved,
                                   ProfileId = achievement.User.Profile.Id,
                                   UserName = achievement.User.UserName,
                                   UserProfileImagePath = achievement.User.Profile.UserImage.FileName
                               };

            var results = await achievements.ToListAsync();

            foreach (var result in results)
            {
                result.UserProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.UserProfileImagePath);
                result.AchievementIconPath = MilestoneExtensions.GetImagePath(result.AchievementIconPath);
            }

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<AchievementFeedViewModel>> GetFollowerAchievementsAsync(string userId)
        {
            var achievements = from achievement in db.MilestoneAchievements
                               join favoritedProfiles in db.FavoriteProfiles on achievement.User.Profile.Id equals favoritedProfiles.ProfileId
                               where favoritedProfiles.UserId == userId
                               where achievement.User.IsActive
                               select new AchievementFeedViewModel()
                               {
                                   AchievementIconPath = achievement.Milestone.IconFileName,
                                   MilestoneAmountRequired = achievement.Milestone.AmountRequired,
                                   MilestoneTypeName = achievement.Milestone.MilestoneType.Name,
                                   DateAchieved = achievement.DateAchieved,
                                   ProfileId = achievement.User.Profile.Id,
                                   UserName = achievement.User.UserName,
                                   UserProfileImagePath = achievement.User.Profile.UserImage.FileName
                               };

            var results = await achievements.ToListAsync();

            foreach (var result in results)
            {
                result.UserProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.UserProfileImagePath);
                result.AchievementIconPath = MilestoneExtensions.GetImagePath(result.AchievementIconPath);
            }

            return results.AsReadOnly();
        }

        #endregion Achievements

        #region Followers

        public async Task<IReadOnlyCollection<FollowerFeedViewModel>> GetFollowersAsync(string currentUserId)
        {
            var followers = from favorite in db.FavoriteProfiles
                            where favorite.Profile.ApplicationUser.Id == currentUserId
                            select new FollowerFeedViewModel()
                            {
                                DateFollowed = favorite.DateFavorited,
                                FollowerName = favorite.User.UserName,
                                FollowerProfileId = favorite.User.Profile.Id,
                                FollowerProfileImagePath = favorite.User.Profile.UserImage.FileName
                            };

            var results = await followers.ToListAsync();

            foreach (var result in results)
            {
                result.FollowerProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.FollowerProfileImagePath);
            }

            return results.AsReadOnly();
        }

        #endregion Followers

        #region Visitors

        public async Task<IReadOnlyCollection<ProfileVisitFeedViewModel>> GetProfileVisitsAsync(string userId)
        {
            var visitors = from visitor in db.ProfileViews
                           where visitor.Profile.ApplicationUser.Id == userId // only where the profile is the user's profile
                           where visitor.ViewerUserId != userId // but don't include visits by the user (he doesn't care if he visited himself)
                           where visitor.ViewerUser.IsActive
                           select new ProfileVisitFeedViewModel()
                           {
                               DateOccurred = visitor.DateVisited,
                               VisitorProfileId = visitor.ViewerUser.Profile.Id,
                               VisitorProfileImagePath = visitor.ViewerUser.Profile.UserImage.FileName,
                               VisitorUserId = visitor.ViewerUserId,
                               VisitorUserName = visitor.ViewerUser.UserName
                           };

            var results = await visitors.ToListAsync();

            // we only want to see a single view for a "time ago"
            // for example, if someone visited a profile 10 times on "October 6", only show a distinct value for that day so the person isn't spammed with notifications
            results = results
                .DistinctBy(x => x.TimeAgo)
                .ToList();

            foreach (var result in results)
            {
                result.VisitorProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.VisitorProfileImagePath);
            }

            return results;
        }

        #endregion
    }
}