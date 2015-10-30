using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class ProfileService : BaseService, IProfileService
    {
        public IUserService UserService { get; set; }

        public ProfileService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        internal static ProfileService Create(IdentityFactoryOptions<ProfileService> options, IOwinContext context)
        {
            var service = new ProfileService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

        /// <summary>
        /// Returns a count of the number of reviews written by a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> GetReviewsWrittenCountByUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            int reviewsWrittenCount = await (from review in db.Reviews
                                             where review.AuthorUserId == userId
                                             select review).CountAsync();

            return reviewsWrittenCount;
        }

        /// <summary>
        /// Returns the current points owned by a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> GetLifetimePointsForUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var points = await (from user in db.Users
                                where user.Id == userId
                                select user.LifetimePoints).FirstOrDefaultAsync();

            return points;
        }

        /// <summary>
        /// Returns a view model object containing a user's complete trivia statistics
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserStatsViewModel> GetUserStatsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            UserStatsViewModel viewModel = new UserStatsViewModel();

            // total points
            // question points + quiz points
            viewModel.TotalPoints = await (from users in db.Users
                                           where users.Id == userId
                                           select users.CurrentPoints).FirstAsync();

            viewModel.QuestionsAnswered =
                await (from answeredQuestion in db.AnsweredQuestions
                       where answeredQuestion.UserId == userId
                       select answeredQuestion).CountAsync();

            viewModel.QuestionsAnsweredCorrectly = await (from answeredQuestion in db.AnsweredQuestions
                                                          join question in db.Questions on answeredQuestion.AnswerId equals question.CorrectAnswerId
                                                          where answeredQuestion.UserId == userId
                                                          select answeredQuestion).CountAsync();

            viewModel.RandomQuestionsAnswered =
                await (from answeredQuestion in db.AnsweredQuestions
                       join question in db.Questions on answeredQuestion.QuestionId equals question.Id
                       where answeredQuestion.UserId == userId
                       where question.QuestionTypeId == (int)QuestionTypeValues.Random
                       select answeredQuestion).CountAsync();

            viewModel.RandomQuestionsAnsweredCorrectly =
                await (from answeredQuestion in db.AnsweredQuestions
                       join question in db.Questions on answeredQuestion.AnswerId equals question.CorrectAnswerId
                       where answeredQuestion.UserId == userId
                       where question.QuestionTypeId == (int)QuestionTypeValues.Random
                       select question).CountAsync();

            viewModel.TimedQuestionsAnswered =
                await (from answeredQuestion in db.AnsweredQuestions
                       join question in db.Questions on answeredQuestion.QuestionId equals question.Id
                       where answeredQuestion.UserId == userId
                       where question.QuestionTypeId == (int)QuestionTypeValues.Timed
                       select answeredQuestion).CountAsync();

            viewModel.TimedQuestionsAnsweredCorrectly =
                await (from answeredQuestion in db.AnsweredQuestions
                       join question in db.Questions on answeredQuestion.AnswerId equals question.CorrectAnswerId
                       where answeredQuestion.UserId == userId
                       where question.QuestionTypeId == (int)QuestionTypeValues.Timed
                       select question).CountAsync();

            viewModel.QuizQuestionsAnswered =
                await (from answeredQuestion in db.AnsweredQuestions
                       join question in db.Questions on answeredQuestion.QuestionId equals question.Id
                       where answeredQuestion.UserId == userId
                       where question.QuestionTypeId == (int)QuestionTypeValues.Quiz
                       select answeredQuestion).CountAsync();

            viewModel.QuizQuestionsAnsweredCorrectly =
                await (from answeredQuestion in db.AnsweredQuestions
                       join question in db.Questions on answeredQuestion.AnswerId equals question.CorrectAnswerId
                       where answeredQuestion.UserId == userId
                       where question.QuestionTypeId == (int)QuestionTypeValues.Quiz
                       select question).CountAsync();

            viewModel.QuizzesCompleted =
                await (from quizCompletions in db.CompletedQuizzes
                       where quizCompletions.UserId == userId
                       select quizCompletions).CountAsync();

            return viewModel;
        }

        /// <summary>
        /// Deletes a user image from the database. Will also detach the image from any profiles that have this selected as their profile image.
        /// </summary>
        /// <param name="userImageId"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteUserImageAsync(int userImageId)
        {
            Debug.Assert(userImageId > 0);

            bool success = false;

            try
            {
                // clear out any profiles that are using this image
                var profilesUsingThisImage = db.Profiles.Where(p => p.UserImageId == userImageId);
                foreach (var profile in profilesUsingThisImage)
                {
                    profile.UserImageId = null;
                }

                // now we can remove the image safely
                UserImage userImage = new UserImage() { Id = userImageId };
                db.UserImages.Attach(userImage);
                db.UserImages.Remove(userImage);

                success = (await db.SaveChangesAsync() > 0);
            }
            catch (DbUpdateException ex)
            {
                Log.Error("ProfileService.DeleteUserImageAsync", ex, new { userImageId = userImageId });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.UserImageNotDeleted);
            }

            return success ? ServiceResult.Success : ServiceResult.Failed(ErrorMessages.UserImageNotDeleted);
        }

        /// <summary>
        /// Deletes a user image from the database. Will also detach the image from any profiles that have this selected as their profile image.
        /// </summary>
        /// <param name="userImageId"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteBannerImageAsync(int userImageId)
        {
            Debug.Assert(userImageId > 0);

            bool success = false;

            try
            {
                // clear out any profiles that are using this image
                var profilesUsingThisImage = db.Profiles.Where(p => p.BannerImageId == userImageId);
                var thisImage = await db.UserImages.FindAsync(userImageId);

                foreach (var profile in profilesUsingThisImage)
                {
                    profile.BannerImageId = null;
                }

                db.UserImages.Remove(thisImage);

                success = (await db.SaveChangesAsync() > 0);
            }
            catch (DbUpdateException ex)
            {
                Log.Error("ProfileService.DeleteBannerImageAsync", ex, new { userImageId = userImageId });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.BannerImageNotDeleted);
            }

            return success ? ServiceResult.Success : ServiceResult.Failed(ErrorMessages.BannerImageNotDeleted);
        }

        public async Task<IReadOnlyCollection<TagAndSuggestedCount>> GetAllTagsAndCountsAsync()
        {
            var tagsAndCounts = from tags in db.TagsAndSuggestedCounts
                                orderby tags.SuggestedCount descending
                                select tags;

            return (await tagsAndCounts.ToListAsync()).AsReadOnly();
        }

        /// <summary>
        /// Returns a collection of all tags in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Tag>> GetAllTagsAsync()
        {
            var tags = from tag in db.Tags
                       orderby tag.Name
                       select tag;

            var result = await tags.ToListAsync();
            return result.AsReadOnly();
        }

        /// <summary>
        /// Returns a collection of view models which contain tags and the count of how many a profile has been awarded.
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<ProfileTagAwardViewModel>> GetTagsAwardedToProfileAsync(int profileId)
        {
            Debug.Assert(profileId > 0);

            var tagsAwarded = from tagAward in db.TagAwards
                              where tagAward.ProfileId == profileId
                              join tag in db.Tags on tagAward.TagId equals tag.TagId
                              group tagAward by new { tagAward.TagId, tag.Name, tagAward.ProfileId, tag.Description }
                                  into grouping
                              select new ProfileTagAwardViewModel()
                              {
                                  TagCount = grouping.Count(),
                                  TagId = grouping.Key.TagId,
                                  TagName = grouping.Key.Name,
                                  TagDescription = grouping.Key.Description
                              };

            var results = await tagsAwarded.ToListAsync();

            return results;
        }

        public async Task<int> GetTagAwardCountForUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var tagAwardCount = await (from tagAward in db.TagAwards
                                       where tagAward.Profile.ApplicationUser.Id == userId
                                       select tagAward).CountAsync();

            return tagAwardCount;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<ProfileTagSuggestionViewModel>> GetTagsSuggestedForProfileAsync(string userId, int profileId)
        {
            Debug.Assert(profileId > 0);

            // i had to separate the queries of identifying if the user suggested and the tag counts because i couldn't find a good way
            // to get counts via group by while maintaining the flag which identifies if the user suggested

            // this will return tag suggestions with an identifier which shows if the passed userId was the one who suggested that tag
            var firstQuery = from tagSuggestion in db.TagSuggestions
                             from tag in db.Tags
                             where tag.TagId == tagSuggestion.TagId
                             where tagSuggestion.ProfileId == profileId
                             where tagSuggestion.SuggestingUser.IsActive // only show suggestions from active users
                             orderby tagSuggestion.DateSuggested descending
                             select new ProfileTagSuggestionViewModel()
                             {
                                 TagId = tag.TagId,
                                 TagName = tag.Name,
                                 TagDescription = tag.Description,
                                 TagCount = 0,
                                 DidUserSuggest = (!String.IsNullOrEmpty(userId) && tagSuggestion.SuggestingUserId == userId) ? true : false
                             };

            // this will return tag suggestions and the count of the number of that tag's suggestions from the first query
            var secondQuery = from f in firstQuery
                              group f by new { f.TagId, f.TagName, f.TagDescription }
                                  into grouping
                              orderby grouping.Count() descending
                              select new ProfileTagSuggestionViewModel()
                              {
                                  TagId = grouping.Key.TagId,
                                  TagName = grouping.Key.TagName,
                                  TagDescription = grouping.Key.TagDescription,
                                  TagCount = grouping.Count(),
                                  DidUserSuggest = false
                              };

            var secondResults = await secondQuery.ToListAsync();

            // now we merge the two results by updated the user suggest flag
            foreach (var tag in secondResults)
            {
                bool didUserSuggest = await firstQuery.AnyAsync(t => t.TagId == tag.TagId && t.DidUserSuggest == true);
                tag.DidUserSuggest = didUserSuggest;
            }

            var results = secondResults.AsReadOnly();

            foreach (var tag in results)
            {
                // get users that suggested the tag for the profile
                tag.SuggestionUsers = await GetUsersWhoSuggestedTagForProfileAsync(tag.TagId, profileId);
            }

            return secondResults.AsReadOnly();
        }

        private async Task<IReadOnlyCollection<TagSuggestionUserViewModel>> GetUsersWhoSuggestedTagForProfileAsync(int tagId, int profileId)
        {
            var users = from tagSuggestion in db.TagSuggestions
                        where tagSuggestion.ProfileId == profileId
                        where tagSuggestion.TagId == tagId
                        select new TagSuggestionUserViewModel()
                        {
                            UserId = tagSuggestion.SuggestingUserId,
                            UserName = tagSuggestion.SuggestingUser.UserName,
                            UserProfileId = tagSuggestion.SuggestingUser.Profile.Id,
                            UserProfileImagePath = tagSuggestion.SuggestingUser.Profile.UserImage != null ? tagSuggestion.SuggestingUser.Profile.UserImage.FileName : String.Empty
                        };

            var results = (await users.Take(6).ToListAsync()).AsReadOnly();

            foreach (var user in results)
            {
                user.UserProfileImagePath = ProfileExtensions.GetThumbnailImagePath(user.UserProfileImagePath);
            }

            return results;
        }

        /// <summary>
        /// Returns a count of how much of a tag is suggested on a profile.
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public async Task<int> GetTagSuggestionCountForProfileAsync(int tagId, int profileId)
        {
            Debug.Assert(tagId > 0);
            Debug.Assert(profileId > 0);

            int count = await (from tagSuggestions in db.TagSuggestions
                               where tagSuggestions.TagId == tagId
                               where tagSuggestions.ProfileId == profileId
                               where tagSuggestions.SuggestingUser.IsActive
                               select tagSuggestions).CountAsync();

            return count;
        }

        /// <summary>
        /// Removes a tag suggestion from a profile
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="profileId"></param>
        /// <param name="suggestingUserId"></param>
        /// <returns></returns>
        public async Task<int> RemoveTagSuggestionAsync(int tagId, int profileId, string suggestingUserId)
        {
            Debug.Assert(tagId > 0);
            Debug.Assert(profileId > 0);
            Debug.Assert(!String.IsNullOrEmpty(suggestingUserId));

            TagSuggestion tagSuggestion = new TagSuggestion()
            {
                ProfileId = profileId,
                SuggestingUserId = suggestingUserId,
                TagId = tagId
            };

            db.TagSuggestions.Attach(tagSuggestion);

            db.TagSuggestions.Remove(tagSuggestion);

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a tag suggestion to a profile.
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="profileId"></param>
        /// <param name="suggestingUserId"></param>
        /// <returns></returns>
        public async Task<int> AddTagSuggestionAsync(int tagId, int profileId, string suggestingUserId)
        {
            Debug.Assert(tagId > 0);
            Debug.Assert(profileId > 0);
            Debug.Assert(!String.IsNullOrEmpty(suggestingUserId));

            TagSuggestion tagSuggestion = new TagSuggestion()
            {
                ProfileId = profileId,
                SuggestingUserId = suggestingUserId,
                TagId = tagId,
                DateSuggested = DateTime.Now
            };

            db.TagSuggestions.Add(tagSuggestion);

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Changes a profile's displayed image.
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="userImageId"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ChangeProfileUserImageAsync(int profileId, int userImageId)
        {
            Debug.Assert(profileId > 0);
            Debug.Assert(userImageId > 0);

            bool success = false;

            try
            {
                var profile = await (from profiles in db.Profiles
                                     where profiles.Id == profileId
                                     select profiles).FirstOrDefaultAsync();

                profile.UserImageId = userImageId;

                success = (await db.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex)
            {
                Log.Error("ProfileService.ChangeProfileUserImageAsync", ex, new { profileId, userImageId });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.ProfileImageNotChanged);
            }

            return success ? ServiceResult.Success : ServiceResult.Failed(ErrorMessages.ProfileImageNotChanged);
        }

        public async Task<int> ChangeProfileBannerImageAsync(int profileId, int userImageId)
        {
            Debug.Assert(profileId > 0);
            Debug.Assert(userImageId > 0);

            var profile = await (from profiles in db.Profiles
                                 where profiles.Id == profileId
                                 select profiles).FirstOrDefaultAsync();

            // delete old and reset
            profile.BannerImageId = null;
            profile.BannerPositionX = 0;
            profile.BannerPositionY = 0;

            // add new
            profile.BannerImageId = userImageId;

            return await db.SaveChangesAsync();
        }

        public async Task<int> SetBannerImagePositionAsync(int profileId, int bannerPositionX, int bannerPositionY)
        {
            Debug.Assert(profileId > 0);

            var profile = await (from profiles in db.Profiles
                                 where profiles.Id == profileId
                                 select profiles).FirstOrDefaultAsync();

            profile.BannerPositionX = bannerPositionX;
            profile.BannerPositionY = bannerPositionY;

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns a collection of a user's uploaded images.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<UserImage>> GetUserImagesAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userImageResult = from userImages in db.UserImages
                                  where userImages.ApplicationUserId == userId
                                  where userImages.IsBanner == false
                                  select userImages;

            var results = await userImageResult.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Adds an uploaded image reference for a user. The Filename references a physical file stored in Azure Storage. Returns the UserImageId of the new image.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<UploadedImageServiceResult> AddUploadedImageForUserAsync(string userId, string fileName, bool isBanner = false)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(!String.IsNullOrEmpty(fileName));

            bool success = false;
            int uploadedImageId = 0;

            try
            {
                UserImage userImage = db.UserImages.Create();
                userImage.ApplicationUserId = userId;
                userImage.FileName = fileName;
                userImage.DateUploaded = DateTime.Now;
                userImage.IsBanner = isBanner;

                db.UserImages.Add(userImage);
                success = (await db.SaveChangesAsync() > 0);

                // don't count banner uploads towards achievements and only try if the upload was successful
                if (success)
                {
                    uploadedImageId = userImage.Id;

                    if (!isBanner)
                    {
                        await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.ProfileImagesUploaded);
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                Log.Error("ProfileService.AddUploadedImageForUserAsync", ex, new { userId, fileName, isBanner });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.UserImageNotUploaded);
            }

            return success ? UploadedImageServiceResult.Success(uploadedImageId) : UploadedImageServiceResult.Failed(ErrorMessages.UserImageNotUploaded);
        }

        /// <summary>
        /// Returns a collection of reviews written for a user. Only reviews written by active users are returned.
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Review>> GetReviewsWrittenForUserAsync(string targetUserId)
        {
            Debug.Assert(!String.IsNullOrEmpty(targetUserId));

            var reviewsForUser = from reviews in db.Reviews
                                 where reviews.TargetUserId == targetUserId
                                 where reviews.AuthorUser.IsActive
                                 select reviews;

            var results = await reviewsForUser.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Writes a review for a user and adds to the user's profile rating. Does nothing if attempting to review self.
        /// </summary>
        /// <param name="authorUserId"></param>
        /// <param name="targetUserId"></param>
        /// <param name="content"></param>
        /// <param name="ratingValue"></param>
        /// <returns></returns>
        public async Task<int> WriteReviewAsync(string authorUserId, string targetUserId, string content, int ratingValue, string authorProfileUrlRoot)
        {
            Debug.Assert(!String.IsNullOrEmpty(authorUserId));
            Debug.Assert(!String.IsNullOrEmpty(targetUserId));
            Debug.Assert(!String.IsNullOrEmpty(content));
            Debug.Assert(ratingValue > 0);

            // don't allow us to send messages to our self
            if (authorUserId == targetUserId)
            {
                return 0;
            }

            Review review = db.Reviews.Create();
            review.AuthorUserId = authorUserId;
            review.TargetUserId = targetUserId;
            review.Content = content;
            review.DateCreated = DateTime.Now;
            review.RatingValue = ratingValue;

            db.Reviews.Add(review);

            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                await AwardAchievedMilestonesForUserAsync(authorUserId, (int)MilestoneTypeValues.ProfileReviewsWritten);

                var authorUser = db.Users.Find(authorUserId);
                string authorUserProfileImagePath = authorUser.Profile.GetThumbnailImagePath();
                string authorUserName = authorUser.UserName;
                string authorProfileUrl = String.Format("{0}/{1}", authorProfileUrlRoot, authorUser.Profile.Id);

                var targetUser = db.Users.Find(targetUserId);
                string targetUserName = targetUser.UserName;
                string targetUserEmail = targetUser.Email;

                await UserService.SendReviewEmailNotificationAsync(authorUserProfileImagePath, authorUserName, content, authorProfileUrl,
                    targetUserId, targetUserName, targetUserEmail);

                await UserService.IncreaseNotificationCountAsync(targetUserId);
            }

            return count;
        }

        /// <summary>
        /// Sends a message from one user to another. Does nothing if attempting to message self.
        /// </summary>
        /// <param name="senderUserId"></param>
        /// <param name="receiverUserId"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<ServiceResult> SendMessageAsync(string senderUserId, string receiverUserId, string body, string conversationUrl)
        {
            Debug.Assert(!String.IsNullOrEmpty(senderUserId));
            Debug.Assert(!String.IsNullOrEmpty(receiverUserId));
            Debug.Assert(!String.IsNullOrEmpty(body));

            // don't allow us to send messages to our self
            if (senderUserId == receiverUserId)
            {
                return ServiceResult.Failed(ErrorMessages.CannotIgnoreSelf);
            }

            bool isReceiverIgnoringSender = await (from ignored in db.IgnoredUsers
                                                   where ignored.SourceUserId == receiverUserId
                                                   where ignored.TargetUserId == senderUserId
                                                   select ignored).AnyAsync();

            // only send the message if the receiver is not ignoring the sender
            if (!isReceiverIgnoringSender)
            {
                CreateMessage(senderUserId, receiverUserId, body);

                bool success = false;

                var senderUser = db.Users.Find(senderUserId);
                if (senderUser.LifetimePoints >= 25)
                {
                    try
                    {
                        success = (await db.SaveChangesAsync() > 0);

                        // message was sent successfully, send email notification
                        if (success)
                        {
                            string senderProfileImagePath = senderUser.Profile.GetThumbnailImagePath();
                            string senderUserName = senderUser.UserName;

                            var receiverUser = db.Users.Find(receiverUserId);
                            string receiverUserName = receiverUser.UserName;
                            string receiverEmail = receiverUser.Email;

                            await UserService.SendMessageEmailNotificationAsync(senderProfileImagePath, senderUserName, body,
                                conversationUrl, receiverUserId, receiverUserName, receiverEmail);

                            await UserService.IncreaseNotificationCountAsync(receiverUserId);
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        Log.Error("ProfileService.SendMessageAsync", ex, new { senderUserId, receiverUserId, body, conversationUrl });
                        ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.MessageNotSent);
                    }

                    return success ? ServiceResult.Success : ServiceResult.Failed(ErrorMessages.MessageNotSent);
                }
                else
                {
                    ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.NeedPointsToMessage);
                    return success ? ServiceResult.Success : ServiceResult.Failed(ErrorMessages.NeedPointsToMessage);
                }
            }
            // if the receiver is ignoring the sender, don't send a message, just pretend we succeeded
            else
            {
                return ServiceResult.Success;
            }
        }

        /// <summary>
        /// Creates a message between two users in the database.
        /// </summary>
        /// <param name="senderUserId"></param>
        /// <param name="receiverUserId"></param>
        /// <param name="body"></param>
        private void CreateMessage(string senderUserId, string receiverUserId, string body)
        {
            Message message = db.Messages.Create();

            message.SenderApplicationUserId = senderUserId;
            message.ReceiverApplicationUserId = receiverUserId;

            message.Body = body;
            message.DateSent = DateTime.Now;
            message.MessageStatusId = (int)MessageStatusValue.Unread;

            db.Messages.Add(message);
        }

        /// <summary>
        /// Returns a profile by its id. Returns null if a profile doesn't exist.
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public async Task<ProfileViewModel> GetProfileAsync(int profileId)
        {
            Debug.Assert(profileId > 0);

            var profile = from profiles in db.Profiles
                          where profiles.Id == profileId
                          select new ProfileViewModel()
                          {
                              IsUserActive = profiles.ApplicationUser.IsActive,
                              UserName = profiles.ApplicationUser.UserName,
                              Birthday = profiles.Birthday,
                              Gender = profiles.Gender.Name,
                              GenderId = profiles.GenderId,
                              CityName = profiles.GeoCity.Name,
                              StateAbbreviation = profiles.GeoCity.GeoState.Abbreviation,
                              CountryName = profiles.GeoCity.GeoState.GeoCountry.Name,
                              ProfileId = profiles.Id,
                              ProfileUserId = profiles.ApplicationUser.Id,
                              ProfileImagePath = profiles.UserImage.FileName,
                              ProfileThumbnailImagePath = profiles.UserImage.FileName,
                              SelectedTitle = profiles.SelectedTitle != null ? profiles.SelectedTitle.Name : String.Empty,
                              SelectedTitleImage = profiles.SelectedTitle != null ? profiles.SelectedTitle.IconFileName : String.Empty,
                              SummaryOfSelf = profiles.SummaryOfSelf,
                              SummaryOfDoing = profiles.SummaryOfDoing,
                              SummaryOfGoing = profiles.SummaryOfGoing,
                              LookingForType = profiles.LookingForType.Name,
                              LookingForLocation = profiles.LookingForLocation.Range,
                              RelationshipStatus = profiles.RelationshipStatus.Name,
                              LookingForAgeMin = profiles.LookingForAgeMin,
                              LookingForAgeMax = profiles.LookingForAgeMax,
                              DateLastLogin = profiles.ApplicationUser.DateLastLogin,
                              BannerImagePath = profiles.BannerImage.FileName,
                              BannerPositionX = profiles.BannerPositionX.HasValue ? profiles.BannerPositionX.Value : 0,
                              BannerPositionY = profiles.BannerPositionY.HasValue ? profiles.BannerPositionY.Value : 0,
                              CurrentPoints = profiles.ApplicationUser.CurrentPoints,
                              LifeTimePoints = profiles.ApplicationUser.LifetimePoints,
                              BannerImageId = profiles.BannerImageId.HasValue ? profiles.BannerImageId.Value : 0,
                              Languages = profiles.Languages.Select(x => x.Name).ToList()
                          };

            var result = await profile.FirstOrDefaultAsync();

            // if the profile doesn't exist, we don't want to try to use it
            if (result != null)
            {
                result.ProfileImagePath = ProfileExtensions.GetImagePath(result.ProfileImagePath);
                result.ProfileThumbnailImagePath = ProfileExtensions.GetThumbnailImagePath(result.ProfileThumbnailImagePath);
                result.SelectedTitleImage = StoreItemExtensions.GetImagePath(result.SelectedTitleImage);
                result.BannerImagePath = UserImageExtensions.GetPath(result.BannerImagePath);
            }

            return result;
        }

        /// <summary>
        /// Returns a profile associated to a user by a user's id. Returns null if no profile exists.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ProfileViewModel> GetProfileAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var profile = from profiles in db.Profiles
                          where profiles.ApplicationUser.Id == userId
                          select new ProfileViewModel()
                          {
                              IsUserActive = profiles.ApplicationUser.IsActive,
                              UserName = profiles.ApplicationUser.UserName,
                              Birthday = profiles.Birthday,
                              Gender = profiles.Gender.Name,
                              GenderId = profiles.GenderId,
                              CityName = profiles.GeoCity.Name,
                              StateAbbreviation = profiles.GeoCity.GeoState.Abbreviation,
                              CountryName = profiles.GeoCity.GeoState.GeoCountry.Name,
                              ProfileId = profiles.Id,
                              ProfileUserId = profiles.ApplicationUser.Id,
                              ProfileImagePath = profiles.UserImage.FileName,
                              ProfileThumbnailImagePath = profiles.UserImage.FileName,
                              SelectedTitle = profiles.SelectedTitle != null ? profiles.SelectedTitle.Name : String.Empty,
                              SelectedTitleImage = profiles.SelectedTitle != null ? profiles.SelectedTitle.IconFileName : String.Empty,
                              SummaryOfSelf = profiles.SummaryOfSelf,
                              SummaryOfDoing = profiles.SummaryOfDoing,
                              SummaryOfGoing = profiles.SummaryOfGoing,
                              LookingForType = profiles.LookingForType.Name,
                              LookingForLocation = profiles.LookingForLocation.Range,
                              RelationshipStatus = profiles.RelationshipStatus.Name,
                              LookingForAgeMin = profiles.LookingForAgeMin,
                              LookingForAgeMax = profiles.LookingForAgeMax,
                              DateLastLogin = profiles.ApplicationUser.DateLastLogin,
                              BannerImagePath = profiles.BannerImage.FileName,
                              BannerPositionX = profiles.BannerPositionX.HasValue ? profiles.BannerPositionX.Value : 0,
                              BannerPositionY = profiles.BannerPositionY.HasValue ? profiles.BannerPositionY.Value : 0,
                              CurrentPoints = profiles.ApplicationUser.CurrentPoints,
                              LifeTimePoints = profiles.ApplicationUser.LifetimePoints,
                              BannerImageId = profiles.BannerImageId.HasValue ? profiles.BannerImageId.Value : 0,
                              Languages = profiles.Languages.Select(x => x.Name).ToList()
                          };

            var result = await profile.FirstOrDefaultAsync();

            result.ProfileImagePath = ProfileExtensions.GetImagePath(result.ProfileImagePath);
            result.ProfileThumbnailImagePath = ProfileExtensions.GetThumbnailImagePath(result.ProfileThumbnailImagePath);
            result.SelectedTitleImage = StoreItemExtensions.GetImagePath(result.SelectedTitleImage);
            result.BannerImagePath = UserImageExtensions.GetPath(result.BannerImagePath);

            return result;
        }

        /// <summary>
        /// Returns a collection of genders.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Gender>> GetGendersAsync()
        {
            var genders = from gender in db.Genders
                          select gender;

            var results = await genders.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Creates a profile based on the entered values. Any location information will be saved to the database since it was originally retrieved via a third party
        /// web service. If that location already exists in our database, we use that instead of inserting.
        /// </summary>
        /// <param name="genderId"></param>
        /// <param name="cityName"></param>
        /// <param name="stateAbbreviation"></param>
        /// <param name="countryName"></param>
        /// <param name="userId"></param>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public async Task<ServiceResult> CreateProfileAsync(int genderId, string cityName, string stateAbbreviation, string countryName, string userId, DateTime birthday)
        {
            Debug.Assert(!String.IsNullOrEmpty(cityName));
            Debug.Assert(!String.IsNullOrEmpty(stateAbbreviation));
            Debug.Assert(!String.IsNullOrEmpty(countryName));
            Debug.Assert(genderId > 0);
            Debug.Assert(!String.IsNullOrEmpty(userId));

            bool success = false;

            try
            {
                int cityId = await GetGeoCityIdAsync(cityName, stateAbbreviation, countryName);

                ApplicationUser user = new ApplicationUser() { Id = userId };

                db.Users.Attach(user);

                Profile newProfile = db.Profiles.Create();
                newProfile.Birthday = birthday;
                newProfile.ApplicationUser = user;
                newProfile.GeoCityId = cityId;
                newProfile.GenderId = genderId;

                db.Profiles.Add(newProfile);
                success = (await db.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex)
            {
                Log.Error("ProfileService.CreateProfileAsync", ex, new { genderId, cityName, stateAbbreviation, countryName, userId, birthday });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.ProfileNotCreated);
            }

            return success ? ServiceResult.Success : ServiceResult.Failed(ErrorMessages.ProfileNotCreated);
        }

        public async Task<int> GetGeoCityIdAsync(string cityName, string stateAbbreviation, string countryName)
        {
            int countryId = await GetGeoCountryIdAsync(countryName);
            int stateId = await GetGeoStateIdAsync(stateAbbreviation, countryId);
            int cityId = await GetGeoCityIdAsync(cityName, stateId);

            return cityId;
        }

        /// <summary>
        /// Returns city id of a location based on city name and state. If the city doesn't exist in the database, it is inserted.
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        private async Task<int> GetGeoCityIdAsync(string cityName, int stateId)
        {
            Debug.Assert(!String.IsNullOrEmpty(cityName));
            Debug.Assert(stateId > 0);

            var city = await db.GeoCities.Where(c => c.Name == cityName && c.GeoStateId == stateId).FirstOrDefaultAsync();

            if (city == null)
            {
                return await CreateNewGeoCity(cityName, stateId);
            }
            else
            {
                return city.Id;
            }
        }

        /// <summary>
        /// Returns state id based on a state abbreviation and country id. If the state doesn't exist in the database, it is inserted.
        /// </summary>
        /// <param name="stateAbbreviation"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        private async Task<int> GetGeoStateIdAsync(string stateAbbreviation, int countryId)
        {
            Debug.Assert(!String.IsNullOrEmpty(stateAbbreviation));
            Debug.Assert(countryId > 0);

            var state = await db.GeoStates.Where(s => s.Abbreviation == stateAbbreviation && s.GeoCountryId == countryId).FirstOrDefaultAsync();

            if (state == null)
            {
                return await CreateNewGeoState(stateAbbreviation, countryId);
            }
            else
            {
                return state.Id;
            }
        }

        /// <summary>
        /// Returns a country id based on a country name. If the country doesn't exist, it is inserted.
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        private async Task<int> GetGeoCountryIdAsync(string countryName)
        {
            Debug.Assert(!String.IsNullOrEmpty(countryName));

            var country = await db.GeoCountries.Where(c => c.Name == countryName).FirstOrDefaultAsync();

            if (country == null)
            {
                return await CreateNewGeoCountry(countryName);
            }
            else
            {
                return country.Id;
            }
        }

        /// <summary>
        /// Creates a new city in the database.
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        private async Task<int> CreateNewGeoCity(string cityName, int stateId)
        {
            Debug.Assert(!String.IsNullOrEmpty(cityName));
            Debug.Assert(stateId > 0);

            GeoCity newGeoCity = new GeoCity()
            {
                Name = cityName,
                GeoStateId = stateId
            };
            db.GeoCities.Add(newGeoCity);
            await db.SaveChangesAsync();
            return newGeoCity.Id;
        }

        /// <summary>
        /// Creates a new state in the database.
        /// </summary>
        /// <param name="stateAbbreviation"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        private async Task<int> CreateNewGeoState(string stateAbbreviation, int countryId)
        {
            Debug.Assert(!String.IsNullOrEmpty(stateAbbreviation));
            Debug.Assert(countryId > 0);

            GeoState newGeoState = new GeoState()
            {
                Abbreviation = stateAbbreviation,
                GeoCountryId = countryId
            };
            db.GeoStates.Add(newGeoState);
            await db.SaveChangesAsync();
            return newGeoState.Id;
        }

        /// <summary>
        /// Creates a new country in the database.
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        private async Task<int> CreateNewGeoCountry(string countryName)
        {
            Debug.Assert(!String.IsNullOrEmpty(countryName));

            GeoCountry newGeoCountry = new GeoCountry()
            {
                Name = countryName
            };
            db.GeoCountries.Add(newGeoCountry);
            await db.SaveChangesAsync();
            return newGeoCountry.Id;
        }

        /// <summary>
        /// Returns a collection of messages sent to and from a user. Returns only messages to and from active users.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Message>> GetMessagesReceivedByUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userMessages = from messages in db.Messages
                               .Include(m => m.ReceiverApplicationUser)
                               .Include(m => m.SenderApplicationUser)
                               .Include(m => m.ReceiverApplicationUser.Profile)
                               .Include(m => m.SenderApplicationUser.Profile)
                               .Include(m => m.ReceiverApplicationUser.Profile.UserImage)
                               .Include(m => m.SenderApplicationUser.Profile.UserImage)
                               where messages.ReceiverApplicationUserId == userId
                               where messages.ReceiverApplicationUser.IsActive
                               where messages.SenderApplicationUser.IsActive
                               select messages;
            var results = await userMessages.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a collection of messages sent to and from a user. Returns only messages to and from active users.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<MessageFeedViewModel>> GetMessagesReceivedByUserFeedAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userMessages = from messages in db.Messages
                               where messages.ReceiverApplicationUserId == userId
                               where messages.ReceiverApplicationUser.IsActive
                               where messages.SenderApplicationUser.IsActive
                               select new MessageFeedViewModel()
                               {
                                   DateOccurred = messages.DateSent,
                                   MessageContent = messages.Body,
                                   ReceiverProfileId = messages.ReceiverApplicationUser.Profile.Id,
                                   ReceiverProfileImagePath = messages.ReceiverApplicationUser.Profile.UserImage.FileName,
                                   ReceiverUserId = messages.ReceiverApplicationUserId,
                                   ReceiverUserName = messages.ReceiverApplicationUser.UserName,
                                   SenderProfileId = messages.SenderApplicationUser.Profile.Id,
                                   SenderProfileImagePath = messages.SenderApplicationUser.Profile.UserImage.FileName,
                                   SenderUserId = messages.SenderApplicationUserId,
                                   SenderUserName = messages.SenderApplicationUser.UserName
                               };

            var results = await userMessages.ToListAsync();

            foreach (var result in results)
            {
                result.ReceiverProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.ReceiverProfileImagePath);
                result.SenderProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.SenderProfileImagePath);
            }

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a collection of messages sent by a user. Returns only messages sent by active users.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Message>> GetMessagesSentByUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userMessages = from messages in db.Messages
                               where messages.SenderApplicationUserId == userId
                               where messages.SenderApplicationUser.IsActive
                               select messages;

            var results = await userMessages.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a collection of message conversations where a user is the sender or the receiver. Only conversations between active users are included.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<MessageConversation>> GetMessageConversationsAsync(string userId)
        {
            var messageConversations = from messages in db.MessageConversations
                                       where messages.SenderApplicationUserId == userId
                                       || messages.ReceiverApplicationUserId == userId
                                       where messages.IsSenderActive
                                       && messages.IsReceiverActive
                                       orderby messages.DateSent descending
                                       select messages;

            var results = await messageConversations.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a collection of messages sent between two users. Only messages to and from active users are included.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userId2"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<ConversationItemViewModel>> GetMessagesBetweenUsersAsync(string userId, string userId2)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(!String.IsNullOrEmpty(userId2));

            var messagesBetweenUsers = from messages in db.Messages
                                       where (messages.SenderApplicationUserId == userId && messages.ReceiverApplicationUserId == userId2)
                                       || (messages.SenderApplicationUserId == userId2 && messages.ReceiverApplicationUserId == userId)
                                       where messages.SenderApplicationUser.IsActive
                                       && messages.ReceiverApplicationUser.IsActive
                                       orderby messages.DateSent descending
                                       select new ConversationItemViewModel()
                                       {
                                           DateSent = messages.DateSent,
                                           MostRecentMessageBody = messages.Body,
                                           MostRecentMessageSenderUserId = messages.SenderApplicationUserId,
                                           MostRecentMessageStatusId = messages.MessageStatusId,
                                           TargetUserId = messages.ReceiverApplicationUserId,
                                           TargetProfileId = messages.ReceiverApplicationUser.Profile.Id,
                                           TargetName = messages.ReceiverApplicationUser.UserName,
                                           TargetProfileImagePath = messages.ReceiverApplicationUser.Profile.UserImage != null
                                            ? messages.ReceiverApplicationUser.Profile.UserImage.FileName
                                            : String.Empty
                                       };


            var results = await messagesBetweenUsers.ToListAsync();

            foreach (var result in results)
            {
                result.TargetProfileImagePath = ProfileExtensions.GetThumbnailImagePath(result.TargetProfileImagePath);
            }

            //// all messages that this user has received in this collection should be marked as read
            //foreach (var message in results)
            //{
            //    if (message.TargetUserId == userId)
            //    {
            //        message.MostRecentMessageStatusId = (int)MessageStatusValue.Read;
            //    }
            //}

            //int changes = await db.SaveChangesAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns the inventory co llection of items for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<InventoryItemViewModel>> GetInventoryAsync(string userId)
        {
            var inventory = from inventoryItems in db.InventoryItems
                            where inventoryItems.ApplicationUserId == userId
                            orderby inventoryItems.ItemCount descending
                            select new InventoryItemViewModel()
                            {
                                GiftDescription = inventoryItems.StoreItem.Description,
                                GiftIconFilePath = inventoryItems.StoreItem.IconFileName,
                                GiftId = inventoryItems.StoreItemId,
                                GiftName = inventoryItems.StoreItem.Name,
                                InventoryItemId = inventoryItems.InventoryItemId,
                                ItemCount = inventoryItems.ItemCount
                            };

            var results = await inventory.ToListAsync();

            foreach (var result in results)
            {
                result.GiftIconFilePath = StoreItemExtensions.GetImagePath(result.GiftIconFilePath);
            }

            return results.AsReadOnly();
        }

        /// <summary>
        /// Sends a gift from a user to another user and adds to the receiving user's inventory.
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="giftId"></param>
        /// <param name="inventoryItemId"></param>
        /// <returns></returns>
        public async Task<int> SendGiftAsync(string fromUserId, string toUserId, int giftId, int inventoryItemId, string senderProfileUrlRoot)
        {
            Debug.Assert(!String.IsNullOrEmpty(fromUserId));
            Debug.Assert(!String.IsNullOrEmpty(toUserId));
            Debug.Assert(giftId > 0);
            Debug.Assert(inventoryItemId > 0);

            // remove the gift from the sender's inventory and add a gift to the receiver's inventory
            int giftCount = await RemoveItemFromUserInventoryAsync(fromUserId, inventoryItemId);
            await AddItemToUserInventoryAsync(toUserId, giftId);

            // save a log of the gift send transaction to the database
            LogGiftTransaction(fromUserId, toUserId, giftId);

            // save any changes in our current transaction
            int changes = await db.SaveChangesAsync();

            if (changes > 0)
            {
                // award achievements if necessary
                await AwardAchievedMilestonesForUserAsync(fromUserId, (int)MilestoneTypeValues.GiftSent);
                await AwardAchievedMilestonesForUserAsync(fromUserId, (int)MilestoneTypeValues.FriendlyExchange);

                // send out email notification
                var senderUser = db.Users.Find(fromUserId);
                string senderProfileImagePath = senderUser.Profile.GetThumbnailImagePath();
                string senderUserName = senderUser.UserName;
                string senderProfileUrl = String.Format("{0}/{1}", senderProfileUrlRoot, senderUser.Profile.Id);

                var receiverUser = db.Users.Find(toUserId);
                string receiverUserName = receiverUser.UserName;
                string receiverEmail = receiverUser.Email;

                var gift = await db.StoreItems.FindAsync(giftId);

                await UserService.SendGiftEmailNotificationAsync(
                    senderUserName, senderProfileImagePath, senderProfileUrl,
                    gift.Name, gift.GetImagePath(),
                    toUserId, receiverUserName, receiverEmail
                );

                await UserService.IncreaseNotificationCountAsync(toUserId);
            }

            return giftCount;
        }

        /// <summary>
        /// Logs a gift being sent between users.
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="giftId"></param>
        private void LogGiftTransaction(string fromUserId, string toUserId, int giftId)
        {
            Debug.Assert(!String.IsNullOrEmpty(fromUserId));
            Debug.Assert(!String.IsNullOrEmpty(toUserId));
            Debug.Assert(giftId > 0);

            // log transaction in gift transaction
            GiftTransactionLog logItem = new GiftTransactionLog()
            {
                DateTransactionOccurred = DateTime.Now,
                FromUserId = fromUserId,
                StoreItemId = giftId,
                ItemCount = 1,
                ToUserId = toUserId,
                IsReviewedByToUser = false
            };
            db.GiftTransactions.Add(logItem);
        }

        /// <summary>
        /// Returns a collection of gift transactions that haven't been "reviewed" by a user. That is, the item will show up in the user's gift received notifications.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<GiftTransactionLog>> GetUnreviewedGiftTransactionsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var results = await (from transactions in db.GiftTransactions
                                 where transactions.ToUserId == userId
                                 where transactions.IsReviewedByToUser == false
                                 select transactions).ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<int> GetSentGiftCountForUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var sentGiftCount = await (from gift in db.GiftTransactions
                                       where gift.FromUserId == userId
                                       select gift).CountAsync();

            return sentGiftCount;
        }

        public async Task<int> GetPurchasedItemCountForUserAsync(string userId, int storeItemTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var purchasedGiftCount = await (from gift in db.StoreTransactions
                                            where gift.UserId == userId
                                            where gift.StoreItem.ItemTypeId == storeItemTypeId
                                            select gift).CountAsync();

            return purchasedGiftCount;
        }

        /// <summary>
        /// Adds
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="giftId"></param>
        /// <returns></returns>
        private async Task<int> AddItemToUserInventoryAsync(string toUserId, int giftId)
        {
            Debug.Assert(!String.IsNullOrEmpty(toUserId));
            Debug.Assert(giftId > 0);

            // increase inventory count for to user id
            var toUserInventoryItem = await (from inventoryItems in db.InventoryItems
                                             where inventoryItems.ApplicationUserId == toUserId
                                             where inventoryItems.StoreItemId == giftId
                                             select inventoryItems).FirstOrDefaultAsync();

            // if there is an inventory item for this gift, increase the count
            if (toUserInventoryItem != null)
            {
                toUserInventoryItem.ItemCount++;
            }
            // else, insert a new inventory item
            else
            {
                InventoryItem newItem = new InventoryItem()
                {
                    ApplicationUserId = toUserId,
                    StoreItemId = giftId,
                    ItemCount = 1
                };

                toUserInventoryItem = db.InventoryItems.Add(newItem);
            }

            return toUserInventoryItem.ItemCount;
        }

        private async Task<int> RemoveItemFromUserInventoryAsync(string fromUserId, int inventoryItemId)
        {
            Debug.Assert(!String.IsNullOrEmpty(fromUserId));
            Debug.Assert(inventoryItemId > 0);

            // reduce inventory count for from user id
            var fromUserInventoryItem = await (from inventoryItems in db.InventoryItems
                                               where inventoryItems.ApplicationUserId == fromUserId
                                               where inventoryItems.InventoryItemId == inventoryItemId
                                               select inventoryItems).FirstOrDefaultAsync();

            // if count > 0, reduce the inventory count by 1
            if (fromUserInventoryItem != null)
            {
                fromUserInventoryItem.ItemCount--;

                // if there's no items left for this inventory item, delete it
                if (fromUserInventoryItem.ItemCount == 0)
                {
                    db.InventoryItems.Remove(fromUserInventoryItem);
                }
            }

            return fromUserInventoryItem.ItemCount;
        }

        /// <summary>
        /// Toggles if a user is favoritng a profile.
        /// </summary>
        /// <param name="followerUserId"></param>
        /// <param name="followingProfileId"></param>
        /// <returns></returns>
        public async Task<bool> ToggleFavoriteProfileAsync(string followerUserId, string followingUserId, int followingProfileId, string profileIndexUrlRoot)
        {
            Debug.Assert(!String.IsNullOrEmpty(followerUserId));
            Debug.Assert(followingProfileId > 0);

            var favoriteEntity = await (from favoriteProfiles in db.FavoriteProfiles
                                        where favoriteProfiles.UserId == followerUserId
                                        where favoriteProfiles.ProfileId == followingProfileId
                                        select favoriteProfiles).FirstOrDefaultAsync();

            bool isFavorite = false;

            // if there is a favorite profile entity match, remove it
            if (favoriteEntity != null)
            {
                db.FavoriteProfiles.Remove(favoriteEntity);

                await db.SaveChangesAsync();
            }
            // else, add it
            else
            {
                FavoriteProfile favoriteProfile = db.FavoriteProfiles.Create();
                favoriteProfile.UserId = followerUserId;
                favoriteProfile.ProfileId = followingProfileId;
                favoriteProfile.DateFavorited = DateTime.Now;
                db.FavoriteProfiles.Add(favoriteProfile);

                bool success = (await db.SaveChangesAsync() > 0);

                isFavorite = true;

                if (success)
                {
                    await SendNewFollowerNotificationAsync(followerUserId, followingProfileId, profileIndexUrlRoot);

                    await UserService.IncreaseNotificationCountAsync(followingUserId);
                }
            }

            return isFavorite;
        }

        private async Task SendNewFollowerNotificationAsync(string followerUserId, int followingProfileId, string profileIndexUrlRoot)
        {
            // the following user is the person who is being followed
            var followingProfile = await db.Profiles.FindAsync(followingProfileId);
            string followingUserName = followingProfile.ApplicationUser.UserName;
            string followingUserEmail = followingProfile.ApplicationUser.Email;
            string followingUserId = followingProfile.ApplicationUser.Id;

            // the follower is the person who did the follow action
            var followerProfile = await db.Profiles.FirstAsync(p => p.ApplicationUser.Id == followerUserId);
            string followerProfileImagePath = followerProfile.GetThumbnailImagePath();
            string followerProfileUrl = String.Format("{0}/{1}", profileIndexUrlRoot, followerProfile.Id);
            string followerUserName = followerProfile.ApplicationUser.UserName;

            await UserService.SendNewFollowerEmailNotificationAsync(followerProfileImagePath, followerUserName, followerProfileUrl,
                followingUserId, followingUserName, followingUserEmail);
        }

        /// <summary>
        /// Toggles if a user is ignoring another user.
        /// </summary>
        /// <param name="sourceUserId"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        public async Task<ToggleServiceResult> ToggleIgnoredUserAsync(string sourceUserId, string targetUserId)
        {
            Debug.Assert(!String.IsNullOrEmpty(sourceUserId));
            Debug.Assert(!String.IsNullOrEmpty(targetUserId));

            bool success = false;
            bool toggleStatus = false;

            try
            {
                var ignoredUserEntity = await (from ignoredUsers in db.IgnoredUsers
                                               where ignoredUsers.SourceUserId == sourceUserId
                                               where ignoredUsers.TargetUserId == targetUserId
                                               select ignoredUsers).FirstOrDefaultAsync();

                // if there is an ignored user entity match, remove it
                if (ignoredUserEntity != null)
                {
                    db.IgnoredUsers.Remove(ignoredUserEntity);

                    // get profile for targetted user
                    var possibleFavoriteProfile = await db.FavoriteProfiles.FirstOrDefaultAsync(t => t.Profile.ApplicationUser.Id == targetUserId);

                    // unfollow that profile
                    if (possibleFavoriteProfile != null)
                    {
                        db.FavoriteProfiles.Remove(possibleFavoriteProfile);
                    }
                }
                // else, add it
                else
                {
                    IgnoredUser ignoredUser = new IgnoredUser()
                    {
                        SourceUserId = sourceUserId,
                        TargetUserId = targetUserId,
                        DateIgnored = DateTime.Now
                    };

                    db.IgnoredUsers.Add(ignoredUser);

                    toggleStatus = true;
                }

                success = (await db.SaveChangesAsync() > 0);
            }
            catch (DbUpdateException ex)
            {
                Log.Error("ProfileService.ToggleIgnoredUserAsync", ex, new { sourceUserId, targetUserId });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.IgnoredUserNotSaved);
            }

            return success ? ToggleServiceResult.Success(toggleStatus) : ToggleServiceResult.Failed(ErrorMessages.IgnoredUserNotSaved);
        }

        /// <summary>
        /// Removes a user's gift notification by markign it as reviewed by a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="giftTransactionId"></param>
        /// <returns></returns>
        public async Task<int> RemoveGiftNotificationAsync(string userId, int giftTransactionId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(giftTransactionId > 0);

            var giftTransaction = await db.GiftTransactions.FindAsync(giftTransactionId);

            if (giftTransaction != null)
            {
                giftTransaction.IsReviewedByToUser = true;
            }

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes all gift notifications for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> RemoveAllGiftNotificationAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var results = await (from giftTransactions in db.GiftTransactions
                                 where giftTransactions.ToUserId == userId
                                 select giftTransactions).ToListAsync();

            foreach (var giftTransaction in results)
            {
                giftTransaction.IsReviewedByToUser = true;
            }

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Sets a user's selected title to be displayed on a profile. Does nothing if the user somehow is setting to a title they haven't earned.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="titleId"></param>
        /// <returns></returns>
        public async Task<int> SetSelectedTitle(string userId, int titleId)
        {
            var user = db.Users.Find(userId);
            bool hasUserObtainedTitle = user.ObtainedTitles.Any(t => t.StoreItemId == titleId);

            // user hasn't obtained this title, don't let them use it
            if (!hasUserObtainedTitle)
            {
                throw new InvalidOperationException(ErrorMessages.TitleNotObtained);
            }

            if (user != null)
            {
                if (user.Profile != null)
                {
                    user.Profile.SelectedTitleId = titleId;
                }
            }

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Logs a visit to a profile by a user.
        /// </summary>
        /// <param name="viewerUserId"></param>
        /// <param name="targetProfileId"></param>
        /// <returns></returns>
        public async Task<int> LogProfileViewAsync(string viewerUserId, int targetProfileId)
        {
            Debug.Assert(!String.IsNullOrEmpty(viewerUserId));
            Debug.Assert(targetProfileId > 0);

            ProfileViewLog newView = new ProfileViewLog()
            {
                ViewerUserId = viewerUserId,
                TargetProfileId = targetProfileId,
                DateVisited = DateTime.Now
            };

            db.ProfileViews.Add(newView);
            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns the gender selected for a profile.
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public async Task<int> GetGenderIdForProfileAsync(int profileId)
        {
            Debug.Assert(profileId > 0);

            var genderId = from profiles in db.Profiles
                           where profiles.Id == profileId
                           select profiles.GenderId;

            return await genderId.FirstAsync();
        }

        /// <summary>
        /// Returns a random set of unique profiles. The set size is determined by the parameter.
        /// </summary>
        /// <param name="numberOfProfilesToRetrieve"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<PersonYouMightAlsoLikeViewModel>> GetRandomProfilesForDashboardAsync(string currentUserId, int numberOfProfilesToRetrieve)
        {
            var allProfiles = await (from profiles in db.Profiles
                                     where profiles.ApplicationUser.IsActive
                                     where profiles.ApplicationUser.Id != currentUserId
                                     where !(from favorite in db.FavoriteProfiles
                                             where favorite.UserId == currentUserId
                                             select favorite.ProfileId)
                                             .Contains(profiles.Id)
                                     select new PersonYouMightAlsoLikeViewModel()
                                     {
                                         UserName = profiles.ApplicationUser.UserName,
                                         Birthday = profiles.Birthday,
                                         LocationCityName = profiles.GeoCity.Name,
                                         LocationStateAbbreviation = profiles.GeoCity.GeoState.Abbreviation,
                                         LocationCountryName = profiles.GeoCity.GeoState.GeoCountry.Name,
                                         ProfileId = profiles.Id,
                                         ProfileThumbnailImagePath = profiles.UserImage.FileName,
                                         ProfileUserId = profiles.ApplicationUser.Id
                                     })
                                     .ToDictionaryAsync(d => d.ProfileId, d => d);

            List<PersonYouMightAlsoLikeViewModel> randomProfiles = new List<PersonYouMightAlsoLikeViewModel>();

            // if there are no profiles, don't do anything
            if (allProfiles == null || allProfiles.Count == 0)
            {
                return randomProfiles.AsReadOnly();
            }

            // if there are less profiles than we want to collect, only collect that amount
            if (allProfiles.Count < numberOfProfilesToRetrieve)
            {
                numberOfProfilesToRetrieve = allProfiles.Count;
            }

            Random random = new Random();
            foreach (var profile in DictionaryHelper.UniqueRandomValues(allProfiles).Take(numberOfProfilesToRetrieve))
            {
                profile.ProfileThumbnailImagePath = ProfileExtensions.GetThumbnailImagePath(profile.ProfileThumbnailImagePath);
                randomProfiles.Add(profile);
            }

            return randomProfiles.AsReadOnly();
        }

        public async Task<int> GetImagesUploadedCountByUserAsync(string userId)
        {
            var imagesUploadedCount = await (from userImages in db.UserImages
                                             where userImages.ApplicationUserId == userId
                                             where userImages.IsBanner == false
                                             select userImages).CountAsync();

            return imagesUploadedCount;
        }

        public async Task<int> GetTagCountAsync(string userId)
        {
            int tagAwardCount = await (from tagAwards in db.TagAwards
                                       where tagAwards.Profile.ApplicationUser.Id == userId
                                       select tagAwards).CountAsync();

            int tagSuggestedCount = await (from tagSuggestions in db.TagSuggestions
                                           where tagSuggestions.Profile.ApplicationUser.Id == userId
                                           select tagSuggestions).CountAsync();

            return tagAwardCount + tagSuggestedCount;
        }

        public async Task<int> GetInventoryCountAsync(string userId)
        {
            int inventoryCount = await (from inventory in db.InventoryItems
                                        where inventory.ApplicationUserId == userId
                                        select (int?)inventory.ItemCount).SumAsync() ?? 0;

            return inventoryCount;
        }

        public async Task<int> SetSelfSummaryAsync(string userId, string selfSummary)
        {
            var profile = db.Users.Find(userId).Profile;
            profile.SummaryOfSelf = selfSummary;
            return await db.SaveChangesAsync();
        }

        public async Task<int> SetSummaryOfDoingAsync(string userId, string summaryOfDoing)
        {
            var profile = db.Users.Find(userId).Profile;
            profile.SummaryOfDoing = summaryOfDoing;
            return await db.SaveChangesAsync();
        }

        public async Task<int> SetSummaryOfGoingAsync(string userId, string summaryOfGoing)
        {
            var profile = db.Users.Find(userId).Profile;
            profile.SummaryOfGoing = summaryOfGoing;
            return await db.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<LookingForType>> GetLookingForTypesAsync()
        {
            var lookingForTypes = from lookingFor in db.LookingForTypes
                                  select lookingFor;

            return (await lookingForTypes.ToListAsync()).AsReadOnly();
        }

        public async Task<IReadOnlyCollection<LookingForLocation>> GetLookingForLocationsAsync()
        {
            var lookingForLocations = from lookingFor in db.LookingForLocations
                                      select lookingFor;

            return (await lookingForLocations.ToListAsync()).AsReadOnly();
        }

        public async Task<int> SetLookingForAsync(string userId, int? lookingForTypeId, int? lookingForLocationId, int? lookingForAgeMin, int? lookingForAgeMax)
        {
            var profile = db.Users.Find(userId).Profile;
            profile.LookingForTypeId = lookingForTypeId;
            profile.LookingForLocationId = lookingForLocationId;
            profile.LookingForAgeMin = lookingForAgeMin;
            profile.LookingForAgeMax = lookingForAgeMax;
            return await db.SaveChangesAsync();
        }

        public async Task<int> SetDetailsAsync(string userId, IReadOnlyCollection<int> languageIds, int? relationshipStatusId)
        {
            var profile = db.Users.Find(userId).Profile;

            profile.RelationshipStatusId = relationshipStatusId;
            await db.Entry<Profile>(profile).Collection(e => e.Languages).LoadAsync();

            profile.Languages.Clear();

            foreach (var languageId in languageIds)
            {
                var language = await db.Languages.FindAsync(languageId);
                profile.Languages.Add(language);
            }

            return await db.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Language>> GetLanguagesAsync()
        {
            var languages = from language in db.Languages
                            select language;

            return (await languages.ToListAsync()).AsReadOnly();
        }

        public async Task<IReadOnlyCollection<RelationshipStatus>> GetRelationshipStatusesAsync()
        {
            var relationshipStatuses = from relationshipStatus in db.RelationshipStatuses
                                       select relationshipStatus;

            return (await relationshipStatuses.ToListAsync()).AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Language>> GetSelectedLanguagesAsync(string currentUserId)
        {
            var languages = from profile in db.Profiles
                            where profile.ApplicationUser.Id == currentUserId
                            from language in profile.Languages
                            select language;

            var results = await languages.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<bool> IsProfileFavoritedByUserAsync(int profileId, string currentUserId)
        {
            Debug.Assert(profileId > 0);

            if (String.IsNullOrEmpty(currentUserId))
            {
                return false;
            }

            var favoritedProfile = await db.FavoriteProfiles.FindAsync(currentUserId, profileId);

            return favoritedProfile != null;
        }

        public async Task<IReadOnlyCollection<SimilarUserViewModel>> GetSimilarProfilesAsync(int profileId)
        {
            Debug.Assert(profileId > 0);

            string sql = @"
select top 6
t.profileid ProfileId,
ui.filename ProfileThumbnailImagePath
from (
	-- all profiles with tag counts that are within the range of comparing user tag count
	select
	tagid,
	profileid
	from dbo.tagawards ta1
	inner join dbo.Profiles p on ta1.ProfileId = p.Id
	inner join dbo.AspNetUsers u on u.Id = p.ApplicationUser_Id
	where tagid in (
		-- all awarded tags that current user has
		select tagid
		from dbo.tagawards ta2
		where profileid = @profileId
		group by tagid
	)
	and profileid <> @profileId
	and u.IsActive = 1
	group by tagid, profileid
	-- exclude where current user tag count is outside range of comparing user tag count
	having (
		select count(tagid)
		from dbo.tagawards ta2
		where profileid = @profileId
		and ta2.tagid = ta1.tagid
		group by tagid
	) > count(tagid) - @rangeMin
	-- exclude where current user tag count is outside range of comparing user tag count
	and (
		select count(tagid)
		from dbo.tagawards ta2
		where profileid = @profileId
		and ta2.tagid = ta1.tagid
		group by tagid
	) < count(tagid) + @rangeMax
) t
inner join dbo.profiles p on t.profileid = p.id
left join dbo.userimages ui on ui.id = p.userimageid
group by t.profileid, ui.filename
order by count(t.profileid) desc";

            var results = await QueryAsync<SimilarUserViewModel>(sql, new { profileId, rangeMin = 2, rangeMax = 2 });

            foreach (var result in results)
            {
                result.ProfileThumbnailImagePath = ProfileExtensions.GetThumbnailImagePath(result.ProfileThumbnailImagePath);
            }

            return results.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<FollowerViewModel>> GetFollowersAsync(int profileId, string userId)
        {
            Debug.Assert(profileId > 0);

            var followers = from favoriteProfile in db.FavoriteProfiles
                            where favoriteProfile.ProfileId == profileId
                            where favoriteProfile.User.IsActive
                            select new FollowerViewModel()
                            {
                                BannerImagePath = favoriteProfile.User.Profile.BannerImage.FileName,
                                BannerPositionX = favoriteProfile.User.Profile.BannerPositionX,
                                BannerPositionY = favoriteProfile.User.Profile.BannerPositionY,
                                ProfileThumbnailImagePath = favoriteProfile.User.Profile.UserImage.FileName,
                                UserName = favoriteProfile.User.Profile.ApplicationUser.UserName,
                                UserSummaryOfSelf = favoriteProfile.User.Profile.SummaryOfSelf,
                                IsFavoritedByCurrentUser = favoriteProfile.User.Profile.FavoritedBy.Any(x => x.UserId == userId),
                                ProfileId = favoriteProfile.User.Profile.Id,
                                UserId = favoriteProfile.User.Id
                            };

            var result = await followers.ToListAsync();

            foreach (var follower in result)
            {
                follower.BannerImagePath = UserImageExtensions.GetPath(follower.BannerImagePath);
                follower.ProfileThumbnailImagePath = ProfileExtensions.GetThumbnailImagePath(follower.ProfileThumbnailImagePath);
            }

            return result.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<FollowerViewModel>> GetFollowingAsync(int profileId, string userId)
        {
            Debug.Assert(profileId > 0);

            var followers = from favoriteProfile in db.FavoriteProfiles
                            where favoriteProfile.User.Profile.Id == profileId
                            where favoriteProfile.Profile.ApplicationUser.IsActive
                            select new FollowerViewModel()
                            {
                                BannerImagePath = favoriteProfile.Profile.BannerImage.FileName,
                                BannerPositionX = favoriteProfile.Profile.BannerPositionX,
                                BannerPositionY = favoriteProfile.Profile.BannerPositionY,
                                ProfileThumbnailImagePath = favoriteProfile.Profile.UserImage.FileName,
                                UserName = favoriteProfile.Profile.ApplicationUser.UserName,
                                UserSummaryOfSelf = favoriteProfile.Profile.SummaryOfSelf,
                                IsFavoritedByCurrentUser = favoriteProfile.Profile.FavoritedBy.Any(x => x.UserId == userId),
                                ProfileId = favoriteProfile.Profile.Id,
                                UserId = favoriteProfile.Profile.ApplicationUser.Id
                            };

            var result = await followers.ToListAsync();

            foreach (var follower in result)
            {
                follower.BannerImagePath = UserImageExtensions.GetPath(follower.BannerImagePath);
                follower.ProfileThumbnailImagePath = ProfileExtensions.GetThumbnailImagePath(follower.ProfileThumbnailImagePath);
            }

            return result.AsReadOnly();
        }

        public async Task<int> GetFollowerCountAsync(int profileId)
        {
            return await db.FavoriteProfiles.CountAsync(x => x.ProfileId == profileId && x.User.IsActive);
        }

        public async Task<int> GetFollowingCountAsync(int profileId)
        {
            return await db.FavoriteProfiles.CountAsync(x => x.User.Profile.Id == profileId && x.Profile.ApplicationUser.IsActive);
        }

        public async Task<QuickProfileViewModel> GetRandomProfileAsync(string userId)
        {
            var profile = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.Id != userId
                                 where profiles.ApplicationUser.IsActive
                                 orderby Guid.NewGuid()
                                 select new QuickProfileViewModel()
                                 {
                                     Birthday = profiles.Birthday,
                                     BannerImagePath = profiles.BannerImage.FileName,
                                     BannerPositionX = profiles.BannerPositionX.HasValue ? profiles.BannerPositionX.Value : 0,
                                     BannerPositionY = profiles.BannerPositionY.HasValue ? profiles.BannerPositionY.Value : 0,
                                     CityName = profiles.GeoCity.Name,
                                     StateAbbreviation = profiles.GeoCity.GeoState.Abbreviation,
                                     CountryName = profiles.GeoCity.GeoState.GeoCountry.Name,
                                     Gender = profiles.Gender.Name,
                                     ProfileId = profiles.Id,
                                     ProfileThumbnailImagePath = profiles.UserImage.FileName,
                                     ProfileUserId = profiles.ApplicationUser.Id,
                                     ReviewCount = 0,//profiles.ApplicationUser.ReceivedReviews != null ? profiles.ApplicationUser.ReceivedReviews.Count  : 0,
                                     AverageRatingValue = 0,//profiles.ApplicationUser.ReceivedReviews != null ? (int)Math.Round(profiles.ApplicationUser.ReceivedReviews.Average(x => x.RatingValue)) : 0,
                                     UserName = profiles.ApplicationUser.UserName
                                 })
                                 .Take(1)
                                 .FirstOrDefaultAsync();

            profile.BannerImagePath = UserImageExtensions.GetPath(profile.BannerImagePath);
            profile.ProfileThumbnailImagePath = ProfileExtensions.GetThumbnailImagePath(profile.ProfileThumbnailImagePath);

            var reviews = await GetReviewsWrittenForUserAsync(profile.ProfileUserId);
            profile.ReviewCount = reviews.Count;
            profile.AverageRatingValue = reviews.AverageRating();

            return profile;
        }

        public async Task<IReadOnlyCollection<GiftTransactionLog>> GetGiftsSentToUsersFromUserAsync(string userId, IEnumerable<string> userIds, TimeSpan duration)
        {
            DateTime start = DateTime.Now.Subtract(duration);

            var time = await (from giftTransactions in db.GiftTransactions
                              where giftTransactions.FromUserId == userId
                              where giftTransactions.DateTransactionOccurred >= start
                              where userIds.Contains(giftTransactions.ToUserId)
                              select giftTransactions)
                              .ToListAsync();

            return time;
        }

        public async Task<IReadOnlyCollection<GiftTransactionLog>> GetGiftsSentToUserAsync(string userId, TimeSpan duration)
        {
            DateTime start = DateTime.Now.Subtract(duration);

            var gifts = await (from giftTransactions in db.GiftTransactions
                               where giftTransactions.ToUserId == userId
                               where giftTransactions.DateTransactionOccurred >= start
                               select giftTransactions).ToListAsync();

            return gifts;
        }
    }
}