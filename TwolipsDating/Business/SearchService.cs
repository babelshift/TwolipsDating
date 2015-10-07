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
    public class SearchService : BaseService, ISearchService
    {
        public SearchService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        public static SearchService Create(IdentityFactoryOptions<SearchService> options, IOwinContext context)
        {
            var service = new SearchService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

        public async Task<IReadOnlyCollection<Profile>> GetProfilesByUserNameAsync(string userName)
        {
            var results = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.UserName.Contains(userName)
                                 where profiles.ApplicationUser.IsActive
                                 select profiles).ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Profile>> GetProfilesByUserNameAsync(string userName, string currentUserId)
        {
            var results = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.UserName.Contains(userName)
                                 where profiles.ApplicationUser.IsActive
                                 where profiles.ApplicationUser.Id != currentUserId
                                 select profiles).ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Profile>> GetProfilesByTagNameAsync(string userName)
        {
            var results = await (from profiles in db.Profiles
                                 join tagSuggestions in db.TagSuggestions on profiles.Id equals tagSuggestions.ProfileId
                                 where tagSuggestions.Tag.Name == userName
                                 where profiles.ApplicationUser.IsActive
                                 select profiles)
                                 .Distinct()
                                 .ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<SearchResultProfileViewModel>> GetProfilesByTagNamesAsync(string[] tags, string userId)
        {
            var profiles = await (from profile in db.Profiles
                            join tagSuggestions in db.TagSuggestions on profile.Id equals tagSuggestions.ProfileId
                            where tags.Contains(tagSuggestions.Tag.Name)
                            where profile.ApplicationUser.IsActive
                            select new SearchResultProfileViewModel()
                            {
                                BannerImagePath = profile.BannerImage.FileName,
                                BannerPositionX = profile.BannerPositionX,
                                BannerPositionY = profile.BannerPositionY,
                                ProfileThumbnailImagePath = profile.UserImage.FileName,
                                UserName = profile.ApplicationUser.UserName,
                                UserSummaryOfSelf = profile.SummaryOfSelf,
                                IsFavoritedByCurrentUser = profile.FavoritedBy.Any(x => x.UserId == userId),
                                ProfileId = profile.Id,
                                UserId = profile.ApplicationUser.Id
                            })
                           .Distinct()
                           .ToListAsync();

            foreach (var profile in profiles)
            {
                profile.BannerImagePath = UserImageExtensions.GetPath(profile.BannerImagePath);
                profile.ProfileThumbnailImagePath = ProfileExtensions.GetProfileThumbnailImagePath(profile.ProfileThumbnailImagePath);
            }

            return profiles.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<QuizOverviewViewModel>> GetQuizzesByTagsAsync(string tag)
        {
            Debug.Assert(!String.IsNullOrEmpty(tag));

            string sql = @"
                select
	                q.Id as Id,
	                q.Name as Name,
	                q.Description as Description,
	                qqc.QuestionPointAverage as AveragePoints,
                    0 as IsComplete,
                    q.ImageFileName as ThumbnailImagePath,
					qc.Id as QuizCategoryId,
					qc.Name as QuizCategoryName
                from
	                dbo.Quizs q
	                inner join
	                (
		                select
		                qq.Quiz_Id as QuizId,
                        round(avg(cast(qu.Points as float)), 0) as QuestionPointAverage
		                from dbo.QuestionQuizs qq
		                inner join dbo.questions qu on qu.Id = qq.Question_Id
		                group by qq.Quiz_Id
	                ) qqc on qqc.QuizId = q.Id
	                inner join dbo.QuestionQuizs as qq on qq.Quiz_Id = q.Id
	                inner join dbo.questions qu on qu.Id = qq.Question_Id
	                inner join dbo.TagQuestions tq on tq.Question_Id = qu.Id
	                inner join dbo.Tags t on t.TagId = tq.Tag_TagId
					inner join dbo.QuizCategories qc on qc.Id = q.QuizCategoryId
                where
	                t.Name in (@tagList)
                group by
	                q.Id,
	                q.Name,
	                q.Description,
                    q.ImageFileName,
	                qqc.QuestionPointAverage,
					qc.Id,
					qc.Name
            ";

            var results = await QueryAsync<QuizOverviewViewModel>(sql, new { tagList = tag });

            foreach(var result in results)
            {
                result.ThumbnailImagePath = QuizExtensions.GetThumbnailImagePath(result.ThumbnailImagePath);
            }

            return results.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<SearchResultProfileViewModel>> GetAllProfilesAsync(string userId)
        {
            var profiles = from profile in db.Profiles
                           where profile.ApplicationUser.IsActive
                           select new SearchResultProfileViewModel()
                           {
                               BannerImagePath = profile.BannerImage.FileName,
                               BannerPositionX = profile.BannerPositionX,
                               BannerPositionY = profile.BannerPositionY,
                               ProfileThumbnailImagePath = profile.UserImage.FileName,
                               UserName = profile.ApplicationUser.UserName,
                               UserSummaryOfSelf = profile.SummaryOfSelf,
                               IsFavoritedByCurrentUser = profile.FavoritedBy.Any(x => x.UserId == userId),
                               ProfileId = profile.Id,
                               UserId = profile.ApplicationUser.Id
                           };

            var result = await profiles.ToListAsync();

            foreach (var profile in result)
            {
                profile.BannerImagePath = UserImageExtensions.GetPath(profile.BannerImagePath);
                profile.ProfileThumbnailImagePath = ProfileExtensions.GetProfileThumbnailImagePath(profile.ProfileThumbnailImagePath);
            }

            return result.AsReadOnly();
        }
    }
}