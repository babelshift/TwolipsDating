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
    public class DashboardService : BaseService, IDashboardService
    {
        public DashboardService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        internal static DashboardService Create(IdentityFactoryOptions<DashboardService> options, IOwinContext context)
        {
            var service = new DashboardService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

        /// <summary>
        /// Returns all uploaded images for profiles that the passed user id has marked as "favorite"
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<UserImage>> GetRecentFollowerImagesAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var imagesUploadedByFavorites = from userImages in db.UserImages
                                            join favoritedProfiles in db.FavoriteProfiles on userImages.ApplicationUser.Profile.Id equals favoritedProfiles.ProfileId
                                            where favoritedProfiles.UserId == userId
                                            where userImages.ApplicationUser.IsActive
                                            where userImages.IsBanner == false
                                            select userImages;

            var results = await imagesUploadedByFavorites.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns all reviews written by and for users marked as favorites for the passed user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Review>> GetRecentFollowerReviewsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            // get the reviews that were authored by favorited users
            var reviewsWrittenByFavorites = from reviews in db.Reviews
                                            join favoritedProfiles in db.FavoriteProfiles on reviews.AuthorUser.Profile.Id equals favoritedProfiles.ProfileId
                                            where favoritedProfiles.UserId == userId
                                            where reviews.TargetUser.IsActive
                                            where reviews.AuthorUser.IsActive
                                            select reviews;

            // get the reviews that were written for the favorited users
            var reviewsWrittenAboutFavorites = from reviews in db.Reviews
                                               join favoritedProfiles in db.FavoriteProfiles on reviews.TargetUser.Profile.Id equals favoritedProfiles.ProfileId
                                               where favoritedProfiles.UserId == userId
                                               where reviews.TargetUser.IsActive
                                               where reviews.AuthorUser.IsActive
                                               select reviews;

            // union the two sets together and return them
            var results = await reviewsWrittenByFavorites.Union(reviewsWrittenAboutFavorites).ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<GiftTransactionLog>> GetRecentFollowerGiftTransactionsAsync(string userId)
        {
            var giftsFromUser = from gifts in db.GiftTransactions
                                .Include(g => g.StoreItem)
                                .Include(g => g.FromUser)
                                join favoritedProfiles in db.FavoriteProfiles on gifts.FromUser.Profile.Id equals favoritedProfiles.ProfileId
                                where favoritedProfiles.UserId == userId
                                where gifts.FromUser.IsActive
                                where gifts.ToUser.IsActive
                                select gifts;

            var giftsToUser = from gifts in db.GiftTransactions
                              .Include(g => g.StoreItem)
                              .Include(g => g.FromUser)
                              join favoritedProfiles in db.FavoriteProfiles on gifts.ToUser.Profile.Id equals favoritedProfiles.ProfileId
                              where favoritedProfiles.UserId == userId
                              where gifts.FromUser.IsActive
                              where gifts.ToUser.IsActive
                              select gifts;

            var results = await giftsFromUser.Union(giftsToUser).ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<CompletedQuizFeedViewModel>> GetRecentFollowerQuizCompletionsAsync(string userId)
        {
            string sql = @"
select
	p.Id SourceProfileId,
	u.UserName SourceUserName,
	ui.FileName SourceProfileImagePath,
	q.Name QuizName,
	q.Id QuizId,
	cq.DateCompleted DateCompleted,
	(
		select count(*)
		from dbo.QuestionQuizs qq2
		inner join dbo.AnsweredQuestions aq on aq.UserId = cq.UserId and aq.QuestionId = qq2.Question_Id
		inner join dbo.Questions qu on qu.Id = aq.QuestionId
		where qq2.Quiz_Id = q.Id
		and aq.AnswerId = qu.CorrectAnswerId
	) CorrectAnswerCount,
	(
		select count(*)
		from dbo.QuestionQuizs qq2
		where qq2.Quiz_Id = q.Id
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

            var results = await QueryAsync<CompletedQuizFeedViewModel>(sql, new { userId = userId });

            foreach (var result in results)
            {
                result.SourceProfileImagePath = ProfileExtensions.GetProfileThumbnailImagePath(result.SourceProfileImagePath);
            }

            return results.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<TagSuggestion>> GetRecentFollowerTagSuggestionsAsync(string userId)
        {
            var tagSuggestionForFavorite = from tagSuggestion in db.TagSuggestions
                                           .Include(t => t.Profile)
                                           .Include(t => t.SuggestingUser)
                                           join favoritedProfiles in db.FavoriteProfiles on tagSuggestion.Profile.Id equals favoritedProfiles.ProfileId
                                           where favoritedProfiles.UserId == userId
                                           where tagSuggestion.SuggestingUser.IsActive
                                           where tagSuggestion.Profile.ApplicationUser.IsActive
                                           select tagSuggestion;

            var tagSuggestionByFavorite = from tagSuggestion in db.TagSuggestions
                                           .Include(t => t.Profile)
                                           .Include(t => t.SuggestingUser)
                                          join favoritedProfiles in db.FavoriteProfiles on tagSuggestion.SuggestingUser.Profile.Id equals favoritedProfiles.ProfileId
                                          where favoritedProfiles.UserId == userId
                                          where tagSuggestion.SuggestingUser.IsActive
                                          where tagSuggestion.Profile.ApplicationUser.IsActive
                                          select tagSuggestion;

            var results = await tagSuggestionForFavorite.Union(tagSuggestionByFavorite).ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<MilestoneAchievement>> GetRecentFollowerAchievementsAsync(string userId)
        {
            var achievements = from achievement in db.MilestoneAchievements
                               .Include(a => a.Milestone)
                               .Include(a => a.User)
                               join favoritedProfiles in db.FavoriteProfiles on achievement.User.Profile.Id equals favoritedProfiles.ProfileId
                               where favoritedProfiles.UserId == userId
                               where achievement.User.IsActive
                               select achievement;

            var results = await achievements.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<FavoriteProfile>> GetRecentFollowersAsync(string currentUserId)
        {
            var followers = from favorite in db.FavoriteProfiles
                            .Include(a => a.User)
                            .Include(a => a.User.Profile)
                            where favorite.Profile.ApplicationUser.Id == currentUserId
                            select favorite;

            var result = await followers.ToListAsync();
            return result.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<GiftTransactionLog>> GetGiftTransactionsForUserAsync(string userId)
        {
            var giftTransactions = from gifts in db.GiftTransactions
                                .Include(g => g.StoreItem)
                                .Include(g => g.FromUser)
                                   where (gifts.FromUserId == userId || gifts.ToUserId == userId)
                                   where gifts.FromUser.IsActive
                                   where gifts.ToUser.IsActive
                                   select gifts;

            var results = await giftTransactions.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<CompletedQuizFeedViewModel>> GetQuizCompletionsForUserAsync(string userId)
        {
            string sql = @"
select
	p.Id SourceProfileId,
	u.UserName SourceUserName,
	ui.FileName SourceProfileImagePath,
	q.Name QuizName,
	q.Id QuizId,
	cq.DateCompleted DateCompleted,
	(
		select count(*)
		from dbo.QuestionQuizs qq2
		inner join dbo.AnsweredQuestions aq on aq.UserId = cq.UserId and aq.QuestionId = qq2.Question_Id
		inner join dbo.Questions qu on qu.Id = aq.QuestionId
		where qq2.Quiz_Id = q.Id
		and aq.AnswerId = qu.CorrectAnswerId
	) CorrectAnswerCount,
	(
		select count(*)
		from dbo.QuestionQuizs qq2
		where qq2.Quiz_Id = q.Id
	) TotalAnswerCount
from dbo.CompletedQuizs cq
inner join dbo.Quizs q on cq.QuizId = q.Id
inner join dbo.AspNetUsers u on cq.UserId = u.Id
inner join dbo.Profiles p on p.ApplicationUser_Id = u.Id
left join dbo.UserImages ui on p.UserImageId = ui.Id
where
	cq.UserId = @userId
	and u.IsActive = 1";

            var results = await QueryAsync<CompletedQuizFeedViewModel>(sql, new { userId = userId });

            foreach (var result in results)
            {
                result.SourceProfileImagePath = ProfileExtensions.GetProfileThumbnailImagePath(result.SourceProfileImagePath);
            }

            return results.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<TagSuggestion>> GetFollowerTagSuggestionsForUserAsync(string userId)
        {
            var tagSuggestionsForUser = from tagSuggestion in db.TagSuggestions
                                           .Include(t => t.Profile)
                                           .Include(t => t.SuggestingUser)
                                        where (tagSuggestion.SuggestingUserId == userId || tagSuggestion.Profile.ApplicationUser.Id == userId)
                                        where tagSuggestion.SuggestingUser.IsActive
                                        where tagSuggestion.Profile.ApplicationUser.IsActive
                                        select tagSuggestion;

            var results = await tagSuggestionsForUser.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<MilestoneAchievement>> GetFollowerAchievementsForUserAsync(string userId)
        {
            var achievements = from achievement in db.MilestoneAchievements
                               .Include(a => a.Milestone)
                               .Include(a => a.User)
                               where achievement.UserId == userId
                               where achievement.User.IsActive
                               select achievement;

            var results = await achievements.ToListAsync();
            return results.AsReadOnly();
        }
    }
}