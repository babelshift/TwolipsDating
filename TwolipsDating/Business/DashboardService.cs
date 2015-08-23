using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class DashboardService : BaseService
    {
        /// <summary>
        /// Returns all uploaded images for profiles that the passed user id has marked as "favorite"
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal async Task<IReadOnlyCollection<UserImage>> GetRecentFollowerImagesAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var imagesUploadedByFavorites = from userImages in db.UserImages
                                            join favoritedProfiles in db.FavoriteProfiles on userImages.ApplicationUser.Profile.Id equals favoritedProfiles.ProfileId
                                            where favoritedProfiles.UserId == userId
                                            where userImages.ApplicationUser.IsActive
                                            select userImages;

            var results = await imagesUploadedByFavorites.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns all reviews written by and for users marked as favorites for the passed user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal async Task<IReadOnlyCollection<Review>> GetRecentFollowerReviewsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            // get the reviews that were authored by favorited users
            var reviewsWrittenByFavorites = from reviews in db.Reviews
                                            join favoritedProfiles in db.FavoriteProfiles on reviews.AuthorUser.Profile.Id equals favoritedProfiles.ProfileId
                                            where favoritedProfiles.UserId == userId
                                            where reviews.AuthorUser.IsActive
                                            select reviews;

            // get the reviews that were written for the favorited users
            var reviewsWrittenAboutFavorites = from reviews in db.Reviews
                                               join favoritedProfiles in db.FavoriteProfiles on reviews.TargetUser.Profile.Id equals favoritedProfiles.ProfileId
                                               where favoritedProfiles.UserId == userId
                                               where reviews.TargetUser.IsActive
                                               select reviews;

            // union the two sets together and return them
            var results = await reviewsWrittenByFavorites.Union(reviewsWrittenAboutFavorites).ToListAsync();
            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<GiftTransactionLog>> GetRecentFollowerGiftTransactionsAsync(string userId)
        {
            var giftsFromUser = from gifts in db.GiftTransactions
                                .Include(g => g.StoreItem)
                                .Include(g => g.FromUser)
                                join favoritedProfiles in db.FavoriteProfiles on gifts.FromUser.Profile.Id equals favoritedProfiles.ProfileId
                                where favoritedProfiles.UserId == userId
                                where gifts.FromUser.IsActive
                                select gifts;

            var giftsToUser = from gifts in db.GiftTransactions
                              .Include(g => g.StoreItem)
                              .Include(g => g.FromUser)
                              join favoritedProfiles in db.FavoriteProfiles on gifts.ToUser.Profile.Id equals favoritedProfiles.ProfileId
                              where favoritedProfiles.UserId == userId
                              where gifts.ToUser.IsActive
                              select gifts;

            var results = await giftsFromUser.Union(giftsToUser).ToListAsync();
            return results.AsReadOnly();
        }
    }
}