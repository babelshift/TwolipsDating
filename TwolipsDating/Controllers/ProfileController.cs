using AutoMapper;
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
        private UserService userService = new UserService();
        private ViolationService violationService = new ViolationService();

        #region Toggle Favorite and Ignore

        [HttpPost]
        public async Task<JsonResult> ToggleFavoriteProfile(string currentUserId, string profileUserId, int profileId)
        {
            try
            {
                // if user is favoriting his own profile, do nothing
                if (currentUserId == profileUserId)
                {
                    return Json(new { success = false });
                }

                // if user is not logged in, forbidden
                // if passed user id is not equal to request current user id, forbidden
                if (!User.Identity.IsAuthenticated || currentUserId != await GetCurrentUserIdAsync())
                {
                    return Json(new { success = false, error = "403 Forbidden" });
                }

                bool isFavorite = await ProfileService.ToggleFavoriteProfileAsync(currentUserId, profileId);

                return Json(new { success = true, isFavorite = isFavorite });
            }
            catch (Exception e)
            {
                Log.Error("ToggleFavoriteProfile", e,
                    parameters: new { currentUserId = currentUserId, profileUserId = profileUserId, profileId = profileId }
                );

                return Json(new { success = false, error = ErrorMessages.FavoriteProfileNotSaved });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ToggleIgnoredUser(string currentUserId, string profileUserId)
        {
            try
            {
                // if user is ignoring himself, do nothing
                if (currentUserId == profileUserId)
                {
                    return Json(new { success = false });
                }

                // if user is not logged in, forbidden
                // if passed user id is not equal to request current user id, forbidden
                if (!User.Identity.IsAuthenticated || currentUserId != await GetCurrentUserIdAsync())
                {
                    return Json(new { success = false, error = "403 Forbidden" });
                }

                bool isIgnored = await ProfileService.ToggleIgnoredUserAsync(currentUserId, profileUserId);

                return Json(new { success = true, isIgnored = isIgnored });
            }
            catch (Exception e)
            {
                Log.Error("ToggleIgnoredUser", e,
                    parameters: new { currentUserId = currentUserId, profileUserId = profileUserId }
                );

                return Json(new { success = false, error = ErrorMessages.IgnoredUserNotSaved });
            }
        }

        #endregion

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
        public async Task<JsonResult> SendGift(string profileUserId, int giftId, int inventoryItemId)
        {
            string currentUserId = await GetCurrentUserIdAsync();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int giftCount = await ProfileService.SendGift(currentUserId, profileUserId, giftId, inventoryItemId);

                    return Json(new { success = true, giftCount = giftCount });
                }
                else
                {
                    return Json(new { success = false, error = ErrorMessages.EmailAddressNotConfirmed });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "SendGift",
                    e,
                    new { currentUserId = currentUserId, profileUserId = profileUserId, giftId = giftId, inventoryItemId = inventoryItemId }
                );

                return Json(new { success = false, error = ErrorMessages.GiftNotSent });
            }
        }

        #endregion

        #region Send Messages

        [HttpPost]
        public async Task<JsonResult> SendMessage(string profileUserId, string messageBody)
        {
            string currentUserId = await GetCurrentUserIdAsync();

            try
            {

                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int changes = await ProfileService.SendMessageAsync(currentUserId, profileUserId, messageBody);

                    if (changes > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        Log.Warn(
                            "SendMessage",
                            ErrorMessages.MessageNotSent,
                            new { currentUserId = currentUserId, profileId = profileUserId, messageBody = messageBody }
                        );

                        return Json(new { success = false, error = ErrorMessages.MessageNotSent });
                    }
                }
                else
                {
                    return Json(new { success = false, error = ErrorMessages.EmailAddressNotConfirmed });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "SendMessage",
                    e,
                    new { currentUserId = currentUserId, profileId = profileUserId, messageBody = messageBody }
                );

                return Json(new { success = false, error = ErrorMessages.MessageNotSent });
            }
        }

        #endregion

        #region Write Reviews

        [HttpPost]
        public async Task<JsonResult> WriteReview(string profileUserId, int rating, string reviewContent)
        {
            try
            {
                string currentUserId = await GetCurrentUserIdAsync();

                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int changes = await ProfileService.WriteReviewAsync(currentUserId, profileUserId, reviewContent, rating);

                    if (changes > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        Log.Warn(
                            "WriteReview",
                            ErrorMessages.ReviewNotSaved,
                            parameters: new { profileId = profileUserId, reviewContent = reviewContent, ratingValue = rating }
                        );

                        return Json(new { success = false, error = ErrorMessages.ReviewNotSaved });
                    }
                }
                else
                {
                    return Json(new { success = false, error = ErrorMessages.EmailAddressNotConfirmed });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "WriteReview",
                    e,
                    parameters: new { profileId = profileUserId, reviewContent = reviewContent, ratingValue = rating }
                );

                return Json(new { success = false, error = ErrorMessages.ReviewNotSaved });
            }
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
                    await SetNotificationsAsync();

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

                await SetNotificationsAsync();

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
            var reviews = await ProfileService.GetReviewsWrittenForUserAsync(profile.ApplicationUser.Id);

            var viewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);
            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);
            viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "feed";
            viewModel.CurrentUserId = currentUserId;
            viewModel.AverageRatingValue = reviews.AverageRating();
            viewModel.ViewMode = ProfileViewModel.ProfileViewMode.ShowProfile;

            // tag suggestions and awards
            var tagsSuggestedForProfile = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, profile.Id);
            viewModel.SuggestedTags = tagsSuggestedForProfile; // these are the tag suggestions that will be displayed at the profile screen
            viewModel.AllTags = await GetAllTagsAndCountsInSystemAsync(tagsSuggestedForProfile); // these are all tags to be displayed in the "suggest" popup
            viewModel.AwardedTags = await ProfileService.GetTagsAwardedToProfileAsync(profile.Id);

            // favorites and ignores
            viewModel.IsFavoritedByCurrentUser = profile.FavoritedBy.Any(f => f.UserId == currentUserId);
            viewModel.IsIgnoredByCurrentUser = await userService.IsUserIgnoredByUserAsync(currentUserId, profile.ApplicationUser.Id);

            // setup the inventory
            var profileInventoryItems = await ProfileService.GetInventoryAsync(profile.ApplicationUser.Id);
            viewModel.Inventory = new ProfileInventoryViewModel();
            viewModel.Inventory.Items = Mapper.Map<IReadOnlyCollection<InventoryItem>, IReadOnlyCollection<InventoryItemViewModel>>(profileInventoryItems);
            viewModel.Inventory.CurrentUserId = currentUserId;
            viewModel.Inventory.ProfileUserId = profile.ApplicationUser.Id;
            var viewerInventoryItems = await ProfileService.GetInventoryAsync(currentUserId);
            viewModel.ViewerInventoryItems = Mapper.Map<IReadOnlyCollection<InventoryItem>, IReadOnlyCollection<InventoryItemViewModel>>(viewerInventoryItems);

            var violationTypes = await violationService.GetViolationTypesAsync();
            viewModel.WriteReviewViolation = new WriteReviewViolationViewModel();
            viewModel.WriteReviewViolation.ViolationTypes = violationTypes.ToDictionary(v => v.Id, v => v.Name);

            // setup viewmodel specific to the actively selected tab
            await SetViewModelBasedOnActiveTabAsync(profile, viewModel, reviews, currentUserId);

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
            string currentUserId)
        {
            if (viewModel.ActiveTab == "feed")
            {
                var userImages = await ProfileService.GetUserImagesAsync(profile.ApplicationUser.Id);
                viewModel.UploadImage = new UploadImageViewModel();
                viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);
                viewModel.Feed = GetUserFeed(profile, reviews, userImages);
                viewModel.Feed.CurrentUserId = currentUserId;
                viewModel.Feed.ProfileUserId = profile.ApplicationUser.Id;
                viewModel.Feed.ProfileUserName = profile.ApplicationUser.UserName;
                viewModel.Feed.ProfileId = profile.Id;
            }
            if (viewModel.ActiveTab == "pictures" || currentUserId == profile.ApplicationUser.Id)
            {
                var userImages = await ProfileService.GetUserImagesAsync(profile.ApplicationUser.Id);
                viewModel.UploadImage = new UploadImageViewModel();
                viewModel.UploadImage.CurrentUserId = currentUserId;
                viewModel.UploadImage.ProfileUserId = profile.ApplicationUser.Id;
                viewModel.UploadImage.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);
                viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);
            }
            if (viewModel.ActiveTab == "reviews")
            {
                // get the user's reviews
                viewModel.Reviews = new ProfileReviewsViewModel();
                viewModel.Reviews.Items = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewViewModel>>(reviews);
                viewModel.Reviews.CurrentUserId = currentUserId;
                viewModel.Reviews.ProfileUserId = profile.ApplicationUser.Id;
                viewModel.Reviews.ProfileUserName = profile.ApplicationUser.UserName;
                viewModel.Reviews.ProfileId = profile.Id;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && userService != null)
            {
                userService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}