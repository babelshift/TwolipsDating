﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class ProfileService : BaseService
    {
        public async Task<int> DeleteUserImage(int userImageId)
        {
            Debug.Assert(userImageId > 0);

            UserImage u = new UserImage() { Id = userImageId };
            db.UserImages.Attach(u);
            db.UserImages.Remove(u);
            return await db.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Tag>> GetAllTagsAsync()
        {
            var tags = from tag in db.Tags
                       select tag;

            var result = await tags.ToListAsync();
            return result.AsReadOnly();
        }

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
                             select new ProfileTagSuggestionViewModel()
                            {
                                TagId = tag.TagId,
                                TagName = tag.Name,
                                TagCount = 0,
                                DidUserSuggest = (!String.IsNullOrEmpty(userId) && tagSuggestion.SuggestingUserId == userId) ? true : false
                            };

            // this will return tag suggestions and the count of the number of that tag's suggestions from the first query
            var secondQuery = from f in firstQuery
                              group f by new { f.TagId, f.TagName }
                                  into grouping
                                  select new ProfileTagSuggestionViewModel()
                                  {
                                      TagId = grouping.Key.TagId,
                                      TagName = grouping.Key.TagName,
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

            return secondResults.AsReadOnly();
        }

        public async Task<int> GetTagSuggestionCountForProfileAsync(int tagId, int profileId)
        {
            Debug.Assert(tagId > 0);
            Debug.Assert(profileId > 0);

            int count = await (from tagSuggestions in db.TagSuggestions
                               where tagSuggestions.TagId == tagId
                               where tagSuggestions.ProfileId == profileId
                               select tagSuggestions).CountAsync();

            return count;
        }

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

        public async Task<IReadOnlyCollection<UserImage>> GetUserImagesAsync(string userId, DateTime startDate)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userImageResult = from userImages in db.UserImages
                                  where userImages.ApplicationUserId == userId
                                  where userImages.DateUploaded >= startDate
                                  select userImages;

            var results = await userImageResult.ToListAsync();
            return results.AsReadOnly();
        }

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

        public async Task<IReadOnlyCollection<UserImage>> GetUserImagesAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userImageResult = from userImages in db.UserImages
                                  where userImages.ApplicationUserId == userId
                                  select userImages;

            var results = await userImageResult.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<int> AddUploadedImageForUserAsync(string userId, string fileName)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(!String.IsNullOrEmpty(fileName));

            UserImage userImage = db.UserImages.Create();
            ApplicationUser user = new ApplicationUser() { Id = userId };
            db.Users.Attach(user);
            userImage.ApplicationUser = user;
            userImage.FileName = fileName;
            userImage.DateUploaded = DateTime.Now;

            db.UserImages.Add(userImage);
            return await db.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Review>> GetReviewsWrittenForUserAsync(string targetUserId)
        {
            Debug.Assert(!String.IsNullOrEmpty(targetUserId));

            var reviewsForUser = from reviews in db.Reviews
                                 where reviews.TargetUserId == targetUserId
                                 select reviews;

            var results = await reviewsForUser.ToListAsync();
            return results.AsReadOnly();
        }

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
            return await db.SaveChangesAsync();
        }

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
            return await db.SaveChangesAsync();
        }

        public async Task<Profile> GetProfileAsync(int profileId)
        {
            Debug.Assert(profileId > 0);

            var profile = from profiles in db.Profiles
                          where profiles.Id == profileId
                          select profiles;

            return await profile.FirstOrDefaultAsync();
        }

        public async Task<Profile> GetUserProfileAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var profile = from user in db.Users
                          where user.Id == userId
                          select user.Profile;

            return await profile.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<Gender>> GetGendersAsync()
        {
            var genders = from gender in db.Genders
                          select gender;

            var results = await genders.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Country>> GetCountriesAsync()
        {
            var countries = from country in db.Countries
                            select country;

            var results = await countries.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<City> GetCityByNameAsync(string cityName)
        {
            Debug.Assert(!String.IsNullOrEmpty(cityName));

            var result = from cities in db.Cities
                         where cities.Name == cityName
                         select cities;
            return await result.FirstOrDefaultAsync();
        }

        public async Task<City> GetCityByZipCodeAsync(string zipCode)
        {
            Debug.Assert(!String.IsNullOrEmpty(zipCode));

            var result = from zipCodes in db.ZipCodes
                         where zipCodes.ZipCodeId == zipCode
                         select zipCodes.City;
            return await result.FirstOrDefaultAsync();
        }

        public async Task<int> CreateProfileAsync(int genderId, int? zipCode, int cityId, string userId, DateTime birthday)
        {
            if (zipCode.HasValue)
            {
                Debug.Assert(zipCode > 0);
            }
            Debug.Assert(genderId > 0);
            Debug.Assert(cityId > 0);
            Debug.Assert(!String.IsNullOrEmpty(userId));

            ApplicationUser user = new ApplicationUser() { Id = userId };

            db.Users.Attach(user);

            Profile p = db.Profiles.Create();
            p.Birthday = birthday;
            p.ApplicationUser = user;
            p.CityId = cityId;
            p.GenderId = genderId;
            p.ZipCode = zipCode;

            db.Profiles.Add(p);
            return await db.SaveChangesAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            int unreadMessageCount = await (from messages in db.Messages
                                            where messages.ReceiverApplicationUserId == userId
                                            where messages.MessageStatusId == (int)MessageStatusValue.Unread
                                            select messages).CountAsync();
            return unreadMessageCount;
        }

        public async Task<IReadOnlyCollection<Message>> GetMessagesByUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var userMessages = from messages in db.Messages
                               where messages.ReceiverApplicationUserId == userId
                               || messages.SenderApplicationUserId == userId
                               select messages;
            var results = await userMessages.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<MessageConversation>> GetMessageConversationsAsync(string userId)
        {
            var messageConversations = from m in db.MessageConversations
                                       where m.SenderApplicationUserId == userId
                                       || m.ReceiverApplicationUserId == userId
                                       orderby m.DateSent descending
                                       select m;

            var results = await messageConversations.ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Message>> GetMessagesBetweenUsersAsync(string userId, string userId2)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(!String.IsNullOrEmpty(userId2));

            var messagesBetweenUsers = from messages in db.Messages
                                       where (messages.SenderApplicationUserId == userId && messages.ReceiverApplicationUserId == userId2)
                                       || (messages.SenderApplicationUserId == userId2 && messages.ReceiverApplicationUserId == userId)
                                       orderby messages.DateSent descending 
                                       select messages;

            var results = await messagesBetweenUsers.ToListAsync();
            return results.AsReadOnly();
        }
    }
}