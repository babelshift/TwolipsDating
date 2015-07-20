using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<IReadOnlyCollection<UserImage>> GetRecentFollowerImagesAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var imagesUploadedByFavorites = from userImages in db.UserImages
                                            join favoritedProfiles in db.FavoriteProfiles on userImages.ApplicationUser.Profile.Id equals favoritedProfiles.ProfileId
                                            where favoritedProfiles.UserId == userId
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

            // get the reviews that were authored by favorites users
            var reviewsWrittenByFavorites = from reviews in db.Reviews
                                            join favoritedProfiles in db.FavoriteProfiles on reviews.AuthorUser.Profile.Id equals favoritedProfiles.ProfileId
                                            where favoritedProfiles.UserId == userId
                                            select reviews;

            // get the reviews that were written for the favorited users
            var reviewsWrittenAboutFavorites = from reviews in db.Reviews
                                               join favoritedProfiles in db.FavoriteProfiles on reviews.TargetUser.Profile.Id equals favoritedProfiles.ProfileId
                                               where favoritedProfiles.UserId == userId
                                               select reviews;

            // union the two sets together and return them
            var results = await reviewsWrittenByFavorites.Union(reviewsWrittenAboutFavorites).ToListAsync();
            return results.AsReadOnly();
        }
    }
}