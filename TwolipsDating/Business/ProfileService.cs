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
    public class ProfileService : BaseService
    {
        public ProfileService() : base() { }

        public ProfileService(ApplicationDbContext db) : base(db) { }

        internal async Task<int> GetReviewsWrittenCountByUserAsync(string userId)
        {
            int reviewsWrittenCount = await (from review in db.Reviews
                                             where review.AuthorUserId == userId
                                             select review).CountAsync();

            return reviewsWrittenCount;
        }

        internal async Task<int> GetPointsForUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var points = await (from user in db.Users
                                select user.Points).FirstOrDefaultAsync();

            return points;
        }

        /// <summary>
        /// Returns a view model object containing a user's complete trivia statistics
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal async Task<UserStatsViewModel> GetUserStatsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            UserStatsViewModel viewModel = new UserStatsViewModel();

            // total points
            // question points + quiz points
            viewModel.TotalPoints = await (from users in db.Users
                                           select users.Points).FirstAsync();

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
        /// Deletes a user image from the database.
        /// </summary>
        /// <param name="userImageId"></param>
        /// <returns></returns>
        internal async Task<int> DeleteUserImage(int userImageId)
        {
            Debug.Assert(userImageId > 0);

            UserImage userImage = new UserImage() { Id = userImageId };
            db.UserImages.Attach(userImage);
            db.UserImages.Remove(userImage);
            return await db.SaveChangesAsync();
        }

        internal async Task<IReadOnlyCollection<TagAndSuggestedCount>> GetAllTagsAndCountsAsync()
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
        internal async Task<IReadOnlyCollection<Tag>> GetAllTagsAsync()
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
        internal async Task<IReadOnlyCollection<ProfileTagAwardViewModel>> GetTagsAwardedToProfileAsync(int profileId)
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

        internal async Task<int> GetTagAwardCountForUserAsync(string userId)
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
        internal async Task<IReadOnlyCollection<ProfileTagSuggestionViewModel>> GetTagsSuggestedForProfileAsync(string userId, int profileId)
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

            foreach(var tag in results)
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

            foreach(var user in results)
            {
                user.UserProfileImagePath = ProfileExtensions.GetProfileImagePath(user.UserProfileImagePath);
            }

            return results;
        }

        /// <summary>
        /// Returns a count of how much of a tag is suggested on a profile.
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        internal async Task<int> GetTagSuggestionCountForProfileAsync(int tagId, int profileId)
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
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task<int> ChangeProfileUserImageAsync(int profileId, int imageId)
        {
            Debug.Assert(profileId > 0);
            Debug.Assert(imageId > 0);

            var profile = await (from profiles in db.Profiles
                                 where profiles.Id == profileId
                                 select profiles).FirstOrDefaultAsync();

            UserImage userImage = new UserImage() { Id = imageId };
            db.UserImages.Attach(userImage);

            profile.UserImage = userImage;

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
                                  select userImages;

            var results = await userImageResult.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Adds an uploaded image reference for a user. The Filename references a physical file stored in Azure Storage.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<int> AddUploadedImageForUserAsync(string userId, string fileName)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(!String.IsNullOrEmpty(fileName));

            UserImage userImage = db.UserImages.Create();
            userImage.ApplicationUserId = userId;
            userImage.FileName = fileName;
            userImage.DateUploaded = DateTime.Now;

            db.UserImages.Add(userImage);
            int count = await db.SaveChangesAsync();

            await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.ProfileImagesUploaded);

            return count;
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
        public async Task<int> WriteReviewAsync(string authorUserId, string targetUserId, string content, int ratingValue)
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

            ApplicationUser authorUser = new ApplicationUser() { Id = authorUserId };
            db.Users.Attach(authorUser);
            review.AuthorUser = authorUser;

            ApplicationUser targetUser = new ApplicationUser() { Id = targetUserId };
            db.Users.Attach(targetUser);
            review.TargetUser = targetUser;

            review.Content = content;
            review.DateCreated = DateTime.Now;
            review.RatingValue = ratingValue;

            db.Reviews.Add(review);

            int count = await db.SaveChangesAsync();

            await AwardAchievedMilestonesForUserAsync(authorUserId, (int)MilestoneTypeValues.ProfileReviewsWritten);

            return count;
        }

        /// <summary>
        /// Sends a message from one user to another. Does nothing if attempting to message self.
        /// </summary>
        /// <param name="senderUserId"></param>
        /// <param name="receiverUserId"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<int> SendMessageAsync(string senderUserId, string receiverUserId, string body)
        {
            Debug.Assert(!String.IsNullOrEmpty(senderUserId));
            Debug.Assert(!String.IsNullOrEmpty(receiverUserId));
            Debug.Assert(!String.IsNullOrEmpty(body));

            // don't allow us to send messages to our self
            if (senderUserId == receiverUserId)
            {
                return 0;
            }

            CreateMessage(senderUserId, receiverUserId, body);

            return await db.SaveChangesAsync();
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

            ApplicationUser senderUser = new ApplicationUser() { Id = senderUserId };
            db.Users.Attach(senderUser);
            message.SenderApplicationUser = senderUser;

            ApplicationUser receiverUser = new ApplicationUser() { Id = receiverUserId };
            db.Users.Attach(receiverUser);
            message.ReceiverApplicationUser = receiverUser;

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
        public async Task<Profile> GetProfileAsync(int profileId)
        {
            Debug.Assert(profileId > 0);

            var profile = from profiles in db.Profiles
                          where profiles.Id == profileId
                          select profiles;

            return await profile.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns a profile associated to a user by a user's id. Returns null if no profile exists.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Profile> GetUserProfileAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var profile = from user in db.Users
                          where user.Id == userId
                          select user.Profile;

            return await profile.FirstOrDefaultAsync();
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
        public async Task<int> CreateProfileAsync(int genderId, string cityName, string stateAbbreviation, string countryName, string userId, DateTime birthday)
        {
            Debug.Assert(!String.IsNullOrEmpty(cityName));
            Debug.Assert(!String.IsNullOrEmpty(stateAbbreviation));
            Debug.Assert(!String.IsNullOrEmpty(countryName));
            Debug.Assert(genderId > 0);
            Debug.Assert(!String.IsNullOrEmpty(userId));

            int countryId = await GetGeoCountryIdAsync(countryName);
            int stateId = await GetGeoStateIdAsync(stateAbbreviation, countryId);
            int cityId = await GetGeoCityIdAsync(cityName, stateId);

            ApplicationUser user = new ApplicationUser() { Id = userId };

            db.Users.Attach(user);

            Profile newProfile = db.Profiles.Create();
            newProfile.Birthday = birthday;
            newProfile.ApplicationUser = user;
            newProfile.GeoCityId = cityId;
            newProfile.GenderId = genderId;

            db.Profiles.Add(newProfile);
            return await db.SaveChangesAsync();
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
        public async Task<IReadOnlyCollection<Message>> GetMessagesByUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userMessages = from messages in db.Messages
                               where messages.ReceiverApplicationUserId == userId
                               || messages.SenderApplicationUserId == userId
                               where messages.ReceiverApplicationUser.IsActive
                               && messages.SenderApplicationUser.IsActive
                               select messages;
            var results = await userMessages.ToListAsync();
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
        /// Returns a collection of messages sent to a user. Returns only messages sent to active users.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Message>> GetMessagesReceivedByUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userMessages = from messages in db.Messages
                               where messages.ReceiverApplicationUserId == userId
                               where messages.ReceiverApplicationUser.IsActive
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
        public async Task<IReadOnlyCollection<Message>> GetMessagesBetweenUsersAsync(string userId, string userId2)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(!String.IsNullOrEmpty(userId2));

            var messagesBetweenUsers = from messages in db.Messages
                                       where (messages.SenderApplicationUserId == userId && messages.ReceiverApplicationUserId == userId2)
                                       || (messages.SenderApplicationUserId == userId2 && messages.ReceiverApplicationUserId == userId)
                                       where messages.SenderApplicationUser.IsActive
                                       && messages.ReceiverApplicationUser.IsActive
                                       orderby messages.DateSent descending
                                       select messages;

            var results = await messagesBetweenUsers.ToListAsync();

            // all messages that this user has received in this collection should be marked as read
            foreach (var message in results)
            {
                if (message.ReceiverApplicationUserId == userId)
                {
                    message.MessageStatusId = (int)MessageStatusValue.Read;
                }
            }

            int changes = await db.SaveChangesAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns the inventory co llection of items for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<InventoryItem>> GetInventoryAsync(string userId)
        {
            var inventory = from inventoryItems in db.InventoryItems
                            where inventoryItems.ApplicationUserId == userId
                            select inventoryItems;

            var results = await inventory.ToListAsync();
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
        public async Task<int> SendGiftAsync(string fromUserId, string toUserId, int giftId, int inventoryItemId)
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
            await db.SaveChangesAsync();

            await AwardAchievedMilestonesForUserAsync(fromUserId, (int)MilestoneTypeValues.GiftSent);

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
        internal async Task<IReadOnlyCollection<GiftTransactionLog>> GetUnreviewedGiftTransactionsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var results = await (from transactions in db.GiftTransactions
                                 where transactions.ToUserId == userId
                                 where transactions.IsReviewedByToUser == false
                                 select transactions).ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<int> GetSentGiftCountForUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var sentGiftCount = await (from gift in db.GiftTransactions
                                       where gift.FromUserId == userId
                                       select gift).CountAsync();

            return sentGiftCount;
        }

        internal async Task<int> GetPurchasedItemCountForUserAsync(string userId, int storeItemTypeId)
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
        /// <param name="currentUserId"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public async Task<bool> ToggleFavoriteProfileAsync(string currentUserId, int profileId)
        {
            Debug.Assert(!String.IsNullOrEmpty(currentUserId));
            Debug.Assert(profileId > 0);

            var favoriteEntity = await (from favoriteProfiles in db.FavoriteProfiles
                                        where favoriteProfiles.UserId == currentUserId
                                        where favoriteProfiles.ProfileId == profileId
                                        select favoriteProfiles).FirstOrDefaultAsync();

            bool isFavorite = false;

            // if there is a favorite profile entity match, remove it
            if (favoriteEntity != null)
            {
                db.FavoriteProfiles.Remove(favoriteEntity);
            }
            // else, add it
            else
            {
                FavoriteProfile favoriteProfile = new FavoriteProfile()
                {
                    UserId = currentUserId,
                    ProfileId = profileId,
                    DateFavorited = DateTime.Now
                };

                db.FavoriteProfiles.Add(favoriteProfile);
                isFavorite = true;
            }

            await db.SaveChangesAsync();

            return isFavorite;
        }

        /// <summary>
        /// Toggles if a user is ignoring another user.
        /// </summary>
        /// <param name="sourceUserId"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        public async Task<bool> ToggleIgnoredUserAsync(string sourceUserId, string targetUserId)
        {
            Debug.Assert(!String.IsNullOrEmpty(sourceUserId));
            Debug.Assert(!String.IsNullOrEmpty(targetUserId));

            var ignoredUserEntity = await (from ignoredUsers in db.IgnoredUsers
                                           where ignoredUsers.SourceUserId == sourceUserId
                                           where ignoredUsers.TargetUserId == targetUserId
                                           select ignoredUsers).FirstOrDefaultAsync();

            bool isIgnored = false;

            // if there is an ignored user entity match, remove it
            if (ignoredUserEntity != null)
            {
                db.IgnoredUsers.Remove(ignoredUserEntity);
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
                isIgnored = true;
            }

            await db.SaveChangesAsync();

            return isIgnored;
        }

        /// <summary>
        /// Removes a user's gift notification by markign it as reviewed by a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="giftTransactionId"></param>
        /// <returns></returns>
        internal async Task<int> RemoveGiftNotificationAsync(string userId, int giftTransactionId)
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
        internal async Task<int> RemoveAllGiftNotificationAsync(string userId)
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
        internal async Task<int> SetSelectedTitle(string userId, int titleId)
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
        internal async Task<int> LogProfileViewAsync(string viewerUserId, int targetProfileId)
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
        internal async Task<int> GetGenderIdForProfileAsync(int profileId)
        {
            Debug.Assert(profileId > 0);

            var genderId = from profiles in db.Profiles
                           where profiles.Id == profileId
                           select profiles.GenderId;

            return await genderId.FirstAsync();
        }

        /// <summary>
        /// Updates a profile selected gender.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="genderId"></param>
        /// <returns></returns>
        internal async Task<int> UpdateGenderAsync(int profileId, int genderId)
        {
            Profile profileUpdated = new Profile()
            {
                Id = profileId
            };

            profileUpdated = db.Profiles.Attach(profileUpdated);
            profileUpdated.GenderId = genderId;

            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns a random set of unique profiles. The set size is determined by the parameter.
        /// </summary>
        /// <param name="profilesToRetrieve"></param>
        /// <returns></returns>
        internal async Task<IReadOnlyCollection<Profile>> GetRandomProfilesForDashboardAsync(string currentUserId, int profilesToRetrieve)
        {
            var allProfiles = await (from profiles in db.Profiles
                                     where profiles.ApplicationUser.IsActive
                                     where profiles.ApplicationUser.Id != currentUserId
                                     where !(from favorite in db.FavoriteProfiles
                                             where favorite.UserId == currentUserId
                                             select favorite.ProfileId)
                                             .Contains(profiles.Id)
                                     select profiles).ToListAsync();

            List<Profile> randomProfiles = new List<Profile>();

            if (allProfiles == null || allProfiles.Count == 0)
            {
                return randomProfiles.AsReadOnly();
            }

            Random random = new Random();
            for (int i = 0; i < profilesToRetrieve; i++)
            {
                // get a random profile and add to our result set
                int indexOfProfileToAdd = random.Next(0, allProfiles.Count);

                var profileToAdd = allProfiles[indexOfProfileToAdd];

                // only add a profile to our results if it's not already in there
                bool isProfileAlreadyAdded = randomProfiles.Any(r => r.Id == profileToAdd.Id);
                if (!isProfileAlreadyAdded)
                {
                    randomProfiles.Add(allProfiles[indexOfProfileToAdd]);
                }
            }

            return randomProfiles.AsReadOnly();
        }

        internal async Task<int> GetImagesUploadedCountByUserAsync(string userId)
        {
            var imagesUploadedCount = await (from image in db.UserImages
                                             where image.ApplicationUserId == userId
                                             select image).CountAsync();

            return imagesUploadedCount;
        }
    }
}