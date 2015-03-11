using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class DashboardService : BaseService
    {
        // get recent uploaded images
        public async Task<IReadOnlyCollection<UserImage>> GetRecentlyUploadedImagesAsync()
        {
            var userImageResult = from userImages in db.UserImages
                                  select userImages;

            var results = await userImageResult.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Review>> GetRecentlyWrittenReviewsAsync()
        {
            var recentReviews = from reviews in db.Reviews
                                 select reviews;

            var results = await recentReviews.ToListAsync();
            return results.AsReadOnly();
        }
    }
}