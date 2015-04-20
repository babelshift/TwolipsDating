﻿using AutoMapper;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class ProfileController : BaseController
    {
        #region Suggest Tags
        
        [HttpPost]
        public async Task<JsonResult> SuggestTag(int id, int profileId, string suggestAction)
        {
            try
            {
                string currentUserId = await GetCurrentUserIdAsync();

                int changes = 0;

                if (suggestAction == "add")
                {
                    changes = await ProfileService.AddTagSuggestionAsync(id, profileId, currentUserId);
                }
                else if (suggestAction == "remove")
                {
                    changes = await ProfileService.RemoveTagSuggestionAsync(id, profileId, currentUserId);
                }

                if (changes > 0)
                {
                    int tagCount = await ProfileService.GetTagSuggestionCountForProfileAsync(id, profileId);
                    return Json(new { tagId = id, success = true, tagCount = tagCount, suggestAction = suggestAction });
                }
                else
                {
                    Log.Warn("SuggestTag", ErrorMessages.TagSuggestionNotSaved,
                        parameters: new { tagId = id, profileId = profileId, suggestAction = suggestAction }
                    );

                    return Json(new { success = false, ErrorMessages.TagSuggestionNotSaved });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error("SuggestTag", e,
                    parameters: new { tagId = id, profileId = profileId, suggestAction = suggestAction }
                );

                return Json(new { success = false, error = ErrorMessages.TagSuggestionNotSaved });
            }
        }

        #endregion

        #region Send Gifts

        [HttpPost]
        public async Task<ActionResult> SendGift(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
            }

            try
            {
                string currentUserId = await GetCurrentUserIdAsync();

                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int changes = await ProfileService.SendGift(currentUserId, viewModel.ProfileUserId, viewModel.SendGift.GiftId, viewModel.SendGift.InventoryItemId);

                    if (changes == 0)
                    {
                        Log.Warn(
                            "SendGift",
                            ErrorMessages.GiftNotSent,
                            new { currentUserId = currentUserId, profileId = viewModel.ProfileUserId, messageBody = viewModel.SendMessage.MessageBody }
                        );

                        AddError(ErrorMessages.GiftNotSent);
                    }
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "SendGift",
                    e,
                    new { profileId = viewModel.ProfileUserId, messageBody = viewModel.SendMessage.MessageBody }
                );

                AddError(ErrorMessages.GiftNotSent);
            }

            return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
        }

        #endregion

        #region Send Messages

        [HttpPost]
        public async Task<ActionResult> SendMessage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
            }

            try
            {
                string currentUserId = await GetCurrentUserIdAsync();

                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int changes = await ProfileService.SendMessageAsync(currentUserId, viewModel.ProfileUserId, viewModel.SendMessage.MessageBody);

                    if (changes == 0)
                    {
                        Log.Warn(
                            "SendMessage",
                            ErrorMessages.MessageNotSent,
                            new { currentUserId = currentUserId, profileId = viewModel.ProfileUserId, messageBody = viewModel.SendMessage.MessageBody }
                        );

                        AddError(ErrorMessages.MessageNotSent);
                    }
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "SendMessage",
                    e,
                    new { profileId = viewModel.ProfileUserId, messageBody = viewModel.SendMessage.MessageBody }
                );

                AddError(ErrorMessages.MessageNotSent);
            }

            return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
        }

        #endregion

        #region Write Reviews

        [HttpPost]
        public async Task<ActionResult> WriteReview(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
            }

            try
            {
                string currentUserId = await GetCurrentUserIdAsync();
                
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int changes = await ProfileService.WriteReviewAsync(currentUserId, viewModel.ProfileUserId, viewModel.WriteReview.ReviewContent, viewModel.WriteReview.RatingValue);

                    if (changes == 0)
                    {
                        Log.Warn(
                            "WriteReview",
                            ErrorMessages.ReviewNotSaved,
                            parameters: new { profileId = viewModel.ProfileUserId, reviewContent = viewModel.WriteReview.ReviewContent, ratingValue = viewModel.WriteReview.RatingValue }
                        );

                        AddError(ErrorMessages.ReviewNotSaved);
                    }
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "WriteReview",
                    e,
                    parameters: new { profileId = viewModel.ProfileUserId, reviewContent = viewModel.WriteReview.ReviewContent, ratingValue = viewModel.WriteReview.RatingValue }
                );

                AddError(ErrorMessages.ReviewNotSaved);
            }

            return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
        }

        #endregion

        #region Manage Image Uploads

        [HttpPost]
        public async Task<ActionResult> DeleteImage(int id, string fileName, string profileUserId)
        {
            string currentUserId = await GetCurrentUserIdAsync();
            bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);
            if (profileUserId != currentUserId || !isCurrentUserEmailConfirmed)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            try
            {
                int changes = await ProfileService.DeleteUserImage(id);

                if (changes > 0)
                {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("twolipsdatingcdn");
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                    await blockBlob.DeleteAsync();

                    return Json(new { success = true });
                }
                else
                {
                    Log.Warn(
                        "DeleteImage",
                        ErrorMessages.UserImageNotDeleted,
                        parameters: new { userImageId = id, fileName = fileName, profileUserId = profileUserId }
                    );

                    return Json(new { success = false, error = ErrorMessages.UserImageNotDeleted });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "DeleteImage",
                    e,
                    parameters: new { userImageId = id, fileName = fileName, profileUserId = profileUserId }
                );

                return Json(new { success = false, error = ErrorMessages.UserImageNotDeleted });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage(UploadImageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = "pictures" });
            }

            string currentUserId = await GetCurrentUserIdAsync();
            bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);
            if (viewModel.ProfileUserId != currentUserId || !isCurrentUserEmailConfirmed)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            foreach (var uploadedImage in viewModel.UploadedImages)
            {
                // skip over non-images
                if (uploadedImage.ContentType != "image/jpeg"
                    && uploadedImage.ContentType != "image/png"
                    && uploadedImage.ContentType != "image/bmp"
                    && uploadedImage.ContentType != "image/gif")
                {
                    continue;
                }

                try
                {
                    string fileType = Path.GetExtension(uploadedImage.FileName);
                    string fileName = String.Format("{0}{1}", Guid.NewGuid(), fileType);

                    int changes = await ProfileService.AddUploadedImageForUserAsync(currentUserId, fileName);

                    if (changes > 0)
                    {
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference("twolipsdatingcdn");
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                        blockBlob.Properties.ContentType = uploadedImage.ContentType;

                        using (var stream = uploadedImage.InputStream)
                        {
                            blockBlob.UploadFromStream(stream);
                        }
                    }
                    else
                    {
                        Log.Warn(
                            "UploadImage",
                            ErrorMessages.UserImageNotUploaded,
                            parameters: new { currentUserId = currentUserId, fileType = fileType, fileName = fileName }
                        );

                        AddError(ErrorMessages.UserImageNotUploaded);
                    }
                }
                catch (DbUpdateException e)
                {
                    Log.Error("UploadImage", e);

                    AddError(ErrorMessages.UserImageNotUploaded);
                }
            }

            return RedirectToIndex(new { tab = "pictures" });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeImage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = viewModel.ActiveTab });
            }

            try
            {
                string currentUserId = await GetCurrentUserIdAsync();
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);
                if (viewModel.ProfileUserId != currentUserId || !isCurrentUserEmailConfirmed)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                int changes = await ProfileService.ChangeProfileUserImageAsync(viewModel.ProfileId, viewModel.ChangeImage.UserImageId);

                if (changes == 0)
                {
                    Log.Warn(
                        "UploadImage", ErrorMessages.ProfileImageNotChanged,
                        new { currentUserId = currentUserId, profileUserId = viewModel.ProfileUserId, userImageId = viewModel.ChangeImage.UserImageId }
                    );

                    AddError(ErrorMessages.ProfileImageNotChanged);
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "UploadImage",
                    e,
                    new { profileUserId = viewModel.ProfileUserId, userImageId = viewModel.ChangeImage.UserImageId }
                );

                AddError(ErrorMessages.ProfileImageNotChanged);
            }

            return RedirectToIndex(new { tab = viewModel.ActiveTab });
        }

        #endregion

        #region Index and Show Profile

        [AllowAnonymous]
        public async Task<ActionResult> Index(int? id = null, string tab = null)
        {
            string currentUserId = await GetCurrentUserIdAsync();

            // user attempting to view profile based on explicit id
            if (id.HasValue)
            {
                Models.Profile profileToBeViewed = await ProfileService.GetProfileAsync(id.Value);

                // user is viewing a valid profile by id
                if (profileToBeViewed != null)
                {
                    return await ShowUserProfileAsync(tab, currentUserId, profileToBeViewed);
                }
                // the profile that the user is viewing doesn't exist
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            // user attempting to view own profile
            else
            {
                // user attempting to view base /profile URL but isn't logged in
                if (String.IsNullOrEmpty(currentUserId))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                Models.Profile currentUserProfile = await ProfileService.GetUserProfileAsync(currentUserId);

                if (currentUserProfile != null)
                {
                    return await ShowUserProfileAsync(tab, currentUserId, currentUserProfile);
                }
                else
                {
                    return await GetViewModelForProfileCreationAsync();
                }
            }
        }

        private async Task<ActionResult> ShowUserProfileAsync(string tab, string currentUserId, Models.Profile profile)
        {
            var tagsSuggestedForProfile = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, profile.Id);
            var reviews = await ProfileService.GetReviewsWrittenForUserAsync(profile.ApplicationUser.Id);

            var viewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);
            viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);
            viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "feed";
            viewModel.CurrentUserId = currentUserId;
            viewModel.AverageRatingValue = reviews.AverageRating();
            viewModel.ViewMode = ProfileViewModel.ProfileViewMode.ShowProfile;
            viewModel.SuggestedTags = tagsSuggestedForProfile; // these are the tag suggestions that will be displayed at the profile screen
            viewModel.AllTags = await GetAllTagsAndCountsInSystemAsync(tagsSuggestedForProfile); // these are all tags to be displayed in the "suggest" popup

            // setup user images and uploads

            // we only want to lookup user images if the user has selected their own profile or the active tab is pictures

            // setup the inventory
            var profileInventoryItems = await ProfileService.GetInventoryAsync(profile.ApplicationUser.Id);
            viewModel.Inventory = new ProfileInventoryViewModel();
            viewModel.Inventory.Items = Mapper.Map<IReadOnlyCollection<InventoryItem>, IReadOnlyCollection<InventoryItemViewModel>>(profileInventoryItems);
            viewModel.Inventory.CurrentUserId = currentUserId;
            viewModel.Inventory.ProfileUserId = profile.ApplicationUser.Id;
            var viewerInventoryItems = await ProfileService.GetInventoryAsync(currentUserId);
            viewModel.ViewerInventoryItems = Mapper.Map<IReadOnlyCollection<InventoryItem>, IReadOnlyCollection<InventoryItemViewModel>>(viewerInventoryItems);

            // setup viewmodel specific to the actively selected tab
            await SetViewModelBasedOnActiveTabAsync(profile, viewModel, reviews, currentUserId, profile.ApplicationUser.Id);

            await SetUnreadCountsInViewBagAsync();

            return View(viewModel);
        }

        private async Task<IReadOnlyCollection<ProfileTagSuggestionViewModel>> GetAllTagsAndCountsInSystemAsync(IReadOnlyCollection<ProfileTagSuggestionViewModel> tagsSuggestedForProfile)
        {
            var allTagsInSystem = Mapper.Map<IReadOnlyCollection<Tag>, IReadOnlyCollection<ProfileTagSuggestionViewModel>>(await ProfileService.GetAllTagsAsync());
            var d = tagsSuggestedForProfile.ToDictionary(t => t.TagId);
            foreach (var tag in allTagsInSystem)
            {
                ProfileTagSuggestionViewModel t = null;
                bool success = d.TryGetValue(tag.TagId, out t);
                if (success)
                {
                    tag.TagCount = t.TagCount;
                    tag.DidUserSuggest = t.DidUserSuggest;
                }
            }
            return allTagsInSystem;
        }

        private async Task SetViewModelBasedOnActiveTabAsync(Models.Profile profile,
            ProfileViewModel viewModel, 
            IReadOnlyCollection<Review> reviews,
            string currentUserId,
            string profileUserId)
        {
            if (viewModel.ActiveTab == "feed")
            {
                var userImages = await ProfileService.GetUserImagesAsync(profileUserId);
                viewModel.UploadImage = new UploadImageViewModel();
                viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);
                viewModel.Feed = GetUserFeed(profile, reviews, userImages);
                viewModel.Feed.CurrentUserId = currentUserId;
                viewModel.Feed.ProfileUserId = profileUserId;
            }
            if (viewModel.ActiveTab == "pictures" || currentUserId == profileUserId)
            {
                var userImages = await ProfileService.GetUserImagesAsync(profileUserId);
                viewModel.UploadImage = new UploadImageViewModel();
                viewModel.UploadImage.CurrentUserId = currentUserId;
                viewModel.UploadImage.ProfileUserId = profile.ApplicationUser.Id;
                viewModel.UploadImage.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);
                viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);
            }
            if (viewModel.ActiveTab == "reviews")
            {
                // get the user's reviews
                viewModel.Reviews = new ProfileReviewsViewModel();
                viewModel.Reviews.Items = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewViewModel>>(reviews);
                viewModel.Reviews.CurrentUserId = currentUserId;
                viewModel.Reviews.ProfileUserId = profileUserId;
            }
        }

        private ProfileFeedViewModel GetUserFeed(Models.Profile profile, IReadOnlyCollection<Review> reviews, IReadOnlyCollection<UserImage> uploadedImages)
        {
            List<ProfileFeedItemViewModel> feedItems = new List<ProfileFeedItemViewModel>();

            var reviewFeedViewModel = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewWrittenFeedViewModel>>(reviews);

            foreach (var reviewFeed in reviewFeedViewModel)
            {
                feedItems.Add(new ProfileFeedItemViewModel()
                {
                    ItemType = DashboardFeedItemType.ReviewWritten,
                    DateOccurred = reviewFeed.DateOccurred,
                    ReviewWrittenFeedItem = reviewFeed
                });
            }

            var uploadedImagesConsolidated = uploadedImages.GetConsolidatedImagesForFeed();

            foreach (var uploadedImage in uploadedImagesConsolidated)
            {
                feedItems.Add(new ProfileFeedItemViewModel()
                {
                    ItemType = DashboardFeedItemType.UploadedPictures,
                    DateOccurred = uploadedImage.DateOccurred,
                    UploadedImageFeedItem = uploadedImage
                });
            }

            var orderedFeed = feedItems.OrderByDescending(v => v.DateOccurred).ToList().AsReadOnly();
            ProfileFeedViewModel viewModel = new ProfileFeedViewModel()
            {
                Items = orderedFeed
            };
            return viewModel;
        }

        private async Task<ActionResult> GetViewModelForProfileCreationAsync()
        {
            var genders = await ProfileService.GetGendersAsync();
            var countries = await ProfileService.GetCountriesAsync();

            ProfileViewModel viewModel = new ProfileViewModel();
            viewModel.ViewMode = ProfileViewModel.ProfileViewMode.CreateProfile;
            viewModel.CreateProfile = new CreateProfileViewModel();

            Dictionary<int, string> genderCollection = new Dictionary<int, string>();
            foreach (var gender in genders)
            {
                genderCollection.Add(gender.Id, gender.Name);
            }

            viewModel.CreateProfile.Genders = genderCollection;

            Dictionary<int, string> countryCollection = new Dictionary<int, string>();
            foreach (var country in countries)
            {
                countryCollection.Add(country.Id, country.Name);
            }

            string currentUserId = await GetCurrentUserIdAsync();
            viewModel.CreateProfile.Countries = countryCollection;
            viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        #endregion

        #region Create Profile

        [HttpPost]
        public async Task<ActionResult> Create(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            string currentUserId = await GetCurrentUserIdAsync();
            DateTime birthday = new DateTime(viewModel.CreateProfile.BirthYear.Value, viewModel.CreateProfile.BirthMonth.Value, viewModel.CreateProfile.BirthDayOfMonth.Value);

            try
            {
                int changes = await ProfileService.CreateProfileAsync(viewModel.CreateProfile.SelectedGenderId.Value, viewModel.CreateProfile.SelectedZipCodeId, viewModel.CreateProfile.SelectedCityId.Value, currentUserId, birthday);

                if (changes == 0)
                {
                    Log.Warn("Create", ErrorMessages.ProfileNotCreated,
                        new
                        {
                            currentUserId = currentUserId,
                            birthday = birthday.ToString(),
                            genderId = viewModel.CreateProfile.SelectedGenderId.Value,
                            zipCodeId = viewModel.CreateProfile.SelectedZipCodeId,
                            cityid = viewModel.CreateProfile.SelectedCityId.Value
                        }
                    );

                    AddError(ErrorMessages.ProfileNotCreated);
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error("Create", e,
                    new
                    {
                        currentUserId = currentUserId,
                        birthday = birthday.ToString(),
                        genderId = viewModel.CreateProfile.SelectedGenderId.Value,
                        zipCodeId = viewModel.CreateProfile.SelectedZipCodeId,
                        cityid = viewModel.CreateProfile.SelectedCityId.Value
                    }
                );

                AddError(ErrorMessages.ProfileNotCreated);
            }

            return RedirectToIndex();
        }

        #endregion
    }
}