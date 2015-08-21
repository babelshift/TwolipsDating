using AutoMapper;
using Microsoft.AspNet.Identity;
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
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class ProfileController : BaseController
    {
        #region Members

        private UserService userService = new UserService();
        private ViolationService violationService = new ViolationService();

        #endregion Members

        #region Toggle Favorite and Ignore

        /// <summary>
        /// Toggles the status of whether or not a user has a profile favorited. Does nothing if the user is attempting to favorite own profile.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="profileUserId"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ToggleFavoriteProfile(string profileUserId, int profileId)
        {
            var currentUserId = User.Identity.GetUserId();

            try
            {
                // if user is favoriting his own profile, do nothing
                if (currentUserId == profileUserId)
                {
                    return Json(new { success = false, error = ErrorMessages.CannotFavoriteOwnProfile });
                }

                bool isFavorite = await ProfileService.ToggleFavoriteProfileAsync(currentUserId, profileId);

                return Json(new { success = true, isFavorite = isFavorite });
            }
            catch (DbUpdateException e)
            {
                Log.Error("ToggleFavoriteProfile", e,
                    parameters: new { currentUserId = currentUserId, profileUserId = profileUserId, profileId = profileId }
                );

                return Json(new { success = false, error = ErrorMessages.FavoriteProfileNotSaved });
            }
        }

        /// <summary>
        /// Toggles the status of whether or not a user has a profile favorited. Does nothing if the user is attempting to favorite own profile.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="profileUserId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ToggleIgnoredUser(string profileUserId)
        {
            var currentUserId = User.Identity.GetUserId();

            try
            {
                // if user is ignoring himself, do nothing
                if (currentUserId == profileUserId)
                {
                    return Json(new { success = false, error = ErrorMessages.CannotIgnoreSelf });
                }

                bool isIgnored = await ProfileService.ToggleIgnoredUserAsync(currentUserId, profileUserId);

                return Json(new { success = true, isIgnored = isIgnored });
            }
            catch (DbUpdateException e)
            {
                Log.Error("ToggleIgnoredUser", e,
                    parameters: new { currentUserId = currentUserId, profileUserId = profileUserId }
                );

                return Json(new { success = false, error = ErrorMessages.IgnoredUserNotSaved });
            }
        }

        #endregion Toggle Favorite and Ignore

        #region Suggest Tags

        /// <summary>
        /// Adds a suggsted tag to a profile based on a users selected action. For example, a user can select "intellectual" tag as a suggestion
        /// for someone else's profile.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileId"></param>
        /// <param name="suggestAction"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SuggestTag(int id, int profileId, string suggestAction)
        {
            try
            {
                string currentUserId = User.Identity.GetUserId();

                int changes = 0;

                if (suggestAction == "add")
                {
                    changes = await ProfileService.AddTagSuggestionAsync(id, profileId, currentUserId);
                }
                else if (suggestAction == "remove")
                {
                    changes = await ProfileService.RemoveTagSuggestionAsync(id, profileId, currentUserId);
                }

                // if no changes occurred as a result of the above functions, then either nothing was changed or the save failed
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

        #endregion Suggest Tags

        #region Send Gifts

        /// <summary>
        /// Sends a gift from a user's inventory to another user's inventory. Does nothing if the current user's email is not confirmed.
        /// </summary>
        /// <param name="profileUserId"></param>
        /// <param name="giftId"></param>
        /// <param name="inventoryItemId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SendGift(string profileUserId, int giftId, int inventoryItemId)
        {
            string currentUserId = User.Identity.GetUserId();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int giftCount = await ProfileService.SendGiftAsync(currentUserId, profileUserId, giftId, inventoryItemId);
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

        /// <summary>
        /// Removes a single gift notification from the notification bar. Does nothing if the current user's email is not confirmed.
        /// </summary>
        /// <param name="giftTransactionId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveGiftNotification(int giftTransactionId)
        {
            string currentUserId = User.Identity.GetUserId();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int changes = await ProfileService.RemoveGiftNotificationAsync(currentUserId, giftTransactionId);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = ErrorMessages.EmailAddressNotConfirmed });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "RemoveGiftNotification",
                    e,
                    new { currentUserId = currentUserId, giftTransactionId = giftTransactionId }
                );

                return Json(new { success = false, error = ErrorMessages.GiftNotificationNotRemoved });
            }
        }

        /// <summary>
        /// Removes all gift notifications for the current logged in user. Does nothing if the current user's email is not confirmed.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveAllGiftNotifications()
        {
            string currentUserId = User.Identity.GetUserId();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int result = await ProfileService.RemoveAllGiftNotificationAsync(currentUserId);

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = ErrorMessages.EmailAddressNotConfirmed });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "RemoveAllGiftNotification",
                    e,
                    new { currentUserId = currentUserId }
                );

                return Json(new { success = false, error = ErrorMessages.AllGiftNotificationsNotRemoved });
            }
        }

        #endregion Send Gifts

        #region Send Messages

        /// <summary>
        /// Sends a message from the currently logged in user to another user. Does nothing if the current user's email is not confirmed.
        /// </summary>
        /// <param name="profileUserId"></param>
        /// <param name="messageBody"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SendMessage(string profileUserId, string messageBody)
        {
            string currentUserId = User.Identity.GetUserId();

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

        #endregion Send Messages

        #region Write Reviews

        /// <summary>
        /// Writes a review for a user from the currently logged in user. Does nothing if the current user's email is not confirmed.
        /// </summary>
        /// <param name="profileUserId"></param>
        /// <param name="rating"></param>
        /// <param name="reviewContent"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> WriteReview(string profileUserId, int rating, string reviewContent)
        {
            try
            {
                string currentUserId = User.Identity.GetUserId();

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

        #endregion Write Reviews

        #region Manage Image Uploads

        /// <summary>
        /// Deletes an uploaded image from a profile. Does nothing if attempting to delete from someone else's profile. Does nothing if the current user's email is not confirmed.
        /// This will delete the image reference in Azure SQL Server and the physical file in Azure Storage Blobs.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <param name="profileUserId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteImage(int id, string fileName, string profileUserId)
        {
            string currentUserId = User.Identity.GetUserId();
            bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

            // look up user that uploaded the image by file name
            // if the user that uploaded the image isn't the current user, reject

            if (profileUserId != currentUserId || !isCurrentUserEmailConfirmed)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            try
            {
                // TODO: how to make this atomic?

                int changes = await ProfileService.DeleteUserImage(id);

                if (changes > 0)
                {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("twolipsdatingcdn");

                    await DeleteFullSizeImageAsync(fileName, container);

                    await DeleteThumbnailImageAsync(fileName, container);

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

        private static async Task DeleteThumbnailImageAsync(string fileName, CloudBlobContainer container)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);
            string thumbnailName = String.Format("{0}_{1}{2}", fileNameWithoutExtension, "thumb", fileExtension);
            CloudBlockBlob blockBlobThumbnail = container.GetBlockBlobReference(thumbnailName);
            await blockBlobThumbnail.DeleteAsync();
        }

        private static async Task DeleteFullSizeImageAsync(string fileName, CloudBlobContainer container)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.DeleteAsync();
        }

        /// <summary>
        /// Uploads multiple images from a user's selection.
        /// Does nothing if attempting to upload to someone else's profile.
        /// Does nothing if the current user's email is not confirmed.
        /// Does nothing if attempting to upload non image files.
        /// This will upload the image reference in Azure SQL Server and the physical file in Azure Storage Blobs.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UploadImage(UploadImageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = "pictures" });
            }

            string currentUserId = User.Identity.GetUserId();

            // user cannot upload an image if they haven't confirmed their email address
            // user cannot upload an image for someone else's profile
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
                    Guid guid = Guid.NewGuid();
                    string fileName = String.Format("{0}{1}", guid, fileType);
                    string fileNameThumb = String.Format("{0}_{2}{1}", guid, fileType, "thumb");

                    // add the image id to our database
                    int changes = await ProfileService.AddUploadedImageForUserAsync(currentUserId, fileName);

                    // if we saved to our database successfully, save to Azure Storage Blob
                    if (changes > 0)
                    {
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference("twolipsdatingcdn");

                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                        blockBlob.Properties.ContentType = uploadedImage.ContentType;

                        // upload the full image and return it in case it had to be resized
                        WebImage image = await UploadFullImageAsync(uploadedImage, blockBlob);

                        // upload the thumbnail of the now possibly resized image
                        await UploadThumbnailAsync(uploadedImage, fileNameThumb, container, image);
                    }
                    else
                    {
                        Log.Warn(
                            "UploadImage", ErrorMessages.UserImageNotUploaded,
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

        private static async Task UploadThumbnailAsync(System.Web.HttpPostedFileBase uploadedImage, string fileNameThumb, CloudBlobContainer container, WebImage image)
        {
            CreateThumbnail(image);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileNameThumb);
            blockBlob.Properties.ContentType = uploadedImage.ContentType;
            byte[] rawImageThumb = image.GetBytes();
            await blockBlob.UploadFromByteArrayAsync(rawImageThumb, 0, rawImageThumb.Length);
        }

        private static async Task<WebImage> UploadFullImageAsync(HttpPostedFileBase uploadedImage, CloudBlockBlob blockBlob)
        {
            WebImage image = new WebImage(uploadedImage.InputStream);
            byte[] rawImage = image.GetBytes();

            // resize the image if it's too big
            if (image.Width > 1000 || image.Height > 1000)
            {
                image = image.Resize(1000, 1000, true);
                rawImage = image.GetBytes();
            }

            await blockBlob.UploadFromByteArrayAsync(rawImage, 0, rawImage.Length);

            return image;
        }

        private static void CreateThumbnail(WebImage image)
        {
            if (image.Width > image.Height)
            {
                int cropAmount = (image.Width - image.Height) / 2;
                image = image.Crop(0, cropAmount, 0, cropAmount);
            }
            else
            {
                int cropAmount = (image.Height - image.Width) / 2;
                image = image.Crop(cropAmount, 0, cropAmount, 0);
            }

            image.Resize(128, 128, true);
        }

        /// <summary>
        /// Changes a user's displayed profile image based on a selected image.
        /// Does nothing if attempting to change someone else's profile.
        /// Does nothing if the current user's email is not confirmed.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ChangeImage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = viewModel.ActiveTab });
            }

            try
            {
                string currentUserId = User.Identity.GetUserId();
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

        #endregion Manage Image Uploads

        #region Index and Show Profile

        /// <summary>
        /// Determines if a user is viewing their own profile, needs to create a profile, or is viewing someone else's profile. If the user is viewing their own profile
        /// or someone else's profile, display it as normal. If the user is viewing their own profile, but it doesn't exist yet, the creation screen is shown.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="seoName"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> Index(int? id = null, string seoName = null, string tab = null)
        {
            string currentUserId = User.Identity.GetUserId();

            // user attempting to view profile based on explicit id
            if (id.HasValue)
            {
                var profileToBeViewed = await ProfileService.GetProfileAsync(id.Value);

                // if the user has a profile and that user isn't inactive, show the profile
                if (profileToBeViewed != null && profileToBeViewed.ApplicationUser.IsActive)
                {
                    string expectedSeoName = ProfileExtensions.GetSEOProfileName(profileToBeViewed.ApplicationUser.UserName);
                    if (seoName != expectedSeoName)
                    {
                        return RedirectToAction("index", new { id = id, seoName = expectedSeoName });
                    }

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
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("login", "account");
                }

                var currentUserProfile = await ProfileService.GetUserProfileAsync(currentUserId);

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

        /// <summary>
        /// Populates a view model containing all data needed to show a user's profile view.
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="currentUserId"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        private async Task<ActionResult> ShowUserProfileAsync(string tab, string currentUserId, Models.Profile profile)
        {
            var reviews = await ProfileService.GetReviewsWrittenForUserAsync(profile.ApplicationUser.Id);

            var viewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);
            viewModel.IsCurrentUserEmailConfirmed = !User.Identity.IsAuthenticated ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);
            viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "feed";
            viewModel.CurrentUserId = currentUserId;
            viewModel.AverageRatingValue = reviews.AverageRating();
            viewModel.ReviewCount = reviews.Count;
            viewModel.ViewMode = ProfileViewModel.ProfileViewMode.ShowProfile;

            // tag suggestions and awards
            await SetupProfileTagSuggestions(currentUserId, profile, viewModel);

            // only do certain things if the viewer of the profile is logged in
            if (User.Identity.IsAuthenticated)
            {
                // setup favorites and ignores
                viewModel.IsFavoritedByCurrentUser = profile.FavoritedBy.Any(f => f.UserId == currentUserId);
                viewModel.IsIgnoredByCurrentUser = await userService.IsUserIgnoredByUserAsync(currentUserId, profile.ApplicationUser.Id);

                // setup inventory for the viewer of the profile
                var viewerInventoryItems = await ProfileService.GetInventoryAsync(currentUserId);
                viewModel.ViewerInventoryItems = Mapper.Map<IReadOnlyCollection<InventoryItem>, IReadOnlyCollection<InventoryItemViewModel>>(viewerInventoryItems);

                // setup violation types
                var violationTypes = await violationService.GetViolationTypesAsync();
                viewModel.WriteReviewViolation = new WriteReviewViolationViewModel();
                viewModel.WriteReviewViolation.ViolationTypes = violationTypes.ToDictionary(v => v.Id, v => v.Name);

                // log a visit to the profile
                await ProfileService.LogProfileViewAsync(currentUserId, viewModel.ProfileId);
            }

            // setup the profile's inventory
            await SetupProfileInventory(currentUserId, profile, viewModel);

            // setup user titles to select (only if the user is viewing their own profile)
            // TODO: optimize this
            if (currentUserId == profile.ApplicationUser.Id)
            {
                var titles = await userService.GetTitlesOwnedByUserAsync(currentUserId);
                List<TitleViewModel> titleViewModel = new List<TitleViewModel>();
                foreach (var title in titles)
                {
                    titleViewModel.Add(new TitleViewModel()
                    {
                        TitleId = title.Key,
                        TitleName = title.Value.StoreItem.Name
                    });
                }
                viewModel.UserTitles = titleViewModel;
            }

            // setup viewmodel specific to the actively selected tab
            await SetViewModelBasedOnActiveTabAsync(profile, viewModel, reviews, currentUserId);

            return View(viewModel);
        }

        /// <summary>
        /// Sets up the view model to include tag suggestions to be displayed and manaaged on a profile.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="profile"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetupProfileTagSuggestions(string currentUserId, Models.Profile profile, ProfileViewModel viewModel)
        {
            var tagsSuggestedForProfile = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, profile.Id);
            viewModel.SuggestedTags = tagsSuggestedForProfile; // these are the tag suggestions that will be displayed at the profile screen
            viewModel.AllTags = await GetAllTagsAndCountsInSystemAsync(tagsSuggestedForProfile); // these are all tags to be displayed in the "suggest" popup
            viewModel.AwardedTags = await ProfileService.GetTagsAwardedToProfileAsync(profile.Id);
        }

        /// <summary>
        /// Sets up the view model to include the profile's inventory items
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="profile"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetupProfileInventory(string currentUserId, Models.Profile profile, ProfileViewModel viewModel)
        {
            var profileInventoryItems = await ProfileService.GetInventoryAsync(profile.ApplicationUser.Id);
            viewModel.Inventory = new ProfileInventoryViewModel();
            viewModel.Inventory.Items = Mapper.Map<IReadOnlyCollection<InventoryItem>, IReadOnlyCollection<InventoryItemViewModel>>(profileInventoryItems);
            viewModel.Inventory.CurrentUserId = currentUserId;
            viewModel.Inventory.ProfileUserId = profile.ApplicationUser.Id;
        }

        /// <summary>
        /// Returns a collection of all tags in the system and a count of how many suggestions a profile has of each.
        /// </summary>
        /// <param name="tagsSuggestedForProfile"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<ProfileTagSuggestionViewModel>> GetAllTagsAndCountsInSystemAsync(IReadOnlyCollection<ProfileTagSuggestionViewModel> tagsSuggestedForProfile)
        {
            var allTags = await ProfileService.GetAllTagsAsync();
            var allTagViewModel = Mapper.Map<IReadOnlyCollection<Tag>, IReadOnlyCollection<ProfileTagSuggestionViewModel>>(allTags);
            var mapOfTags = tagsSuggestedForProfile.ToDictionary(t => t.TagId);

            // count each tag that has been suggsted for this profile and mark if the tag has already been suggested by the current user
            foreach (var tag in allTagViewModel)
            {
                ProfileTagSuggestionViewModel suggestedTag = null;
                bool success = mapOfTags.TryGetValue(tag.TagId, out suggestedTag);
                if (success)
                {
                    tag.TagCount = suggestedTag.TagCount;
                    tag.DidUserSuggest = suggestedTag.DidUserSuggest;
                }
            }
            return allTagViewModel;
        }

        /// <summary>
        /// Sets some extra items on the view model based on the tab that the user has selected. For example, if the user selects to view the feed, then extra
        /// items will need to be populated such as the items that are displayed in the feed.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="viewModel"></param>
        /// <param name="reviews"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
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
                viewModel.Reviews = new ProfileReviewsViewModel();
                viewModel.Reviews.Items = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewViewModel>>(reviews);
                viewModel.Reviews.CurrentUserId = currentUserId;
                viewModel.Reviews.ProfileUserId = profile.ApplicationUser.Id;
                viewModel.Reviews.ProfileUserName = profile.ApplicationUser.UserName;
                viewModel.Reviews.ProfileId = profile.Id;
            }
        }

        /// <summary>
        /// Returns the profile feed view model which is used to display all feed items that have occurred for a profile including reviews, messages, and uploaded images.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="reviews"></param>
        /// <param name="uploadedImages"></param>
        /// <returns></returns>
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

        /// <summary>
        /// If the profile doesn't exist when the user wants to view their own profile, use a special view model which only allows for the creation of a profile.
        /// </summary>
        /// <returns></returns>
        private async Task<ActionResult> GetViewModelForProfileCreationAsync()
        {
            var genders = await ProfileService.GetGendersAsync();

            ProfileViewModel viewModel = new ProfileViewModel();
            viewModel.ViewMode = ProfileViewModel.ProfileViewMode.CreateProfile;
            viewModel.CreateProfile = new CreateProfileViewModel();

            Dictionary<int, string> genderCollection = new Dictionary<int, string>();
            foreach (var gender in genders)
            {
                genderCollection.Add(gender.Id, gender.Name);
            }

            viewModel.CreateProfile.Genders = genderCollection;

            string currentUserId = User.Identity.GetUserId();
            viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        #endregion Index and Show Profile

        #region Create Profile

        [HttpPost]
        public async Task<ActionResult> Create(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            string currentUserId = User.Identity.GetUserId();
            DateTime birthday = new DateTime(viewModel.CreateProfile.BirthYear.Value, viewModel.CreateProfile.BirthMonth.Value, viewModel.CreateProfile.BirthDayOfMonth.Value);

            string[] location = viewModel.CreateProfile.SelectedLocation.Split(',');
            string cityName = location[0].Trim();
            string stateAbbreviation = location[1].Trim();
            string countryName = location[2].Trim();

            try
            {
                int changes = await ProfileService.CreateProfileAsync(
                    viewModel.CreateProfile.SelectedGenderId.Value,
                    cityName,
                    stateAbbreviation,
                    countryName,
                    currentUserId, birthday);

                if (changes == 0)
                {
                    Log.Warn("Create", ErrorMessages.ProfileNotCreated,
                        new
                        {
                            currentUserId = currentUserId,
                            birthday = birthday.ToString(),
                            genderId = viewModel.CreateProfile.SelectedGenderId.Value,
                            cityName = cityName,
                            stateAbbreviation = stateAbbreviation,
                            countryName = countryName
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
                        cityName = cityName,
                        stateAbbreviation = stateAbbreviation,
                        countryName = countryName
                    }
                );

                AddError(ErrorMessages.ProfileNotCreated);
            }

            return RedirectToIndex();
        }

        #endregion Create Profile

        #region Select Title

        /// <summary>
        /// Sets a displayed profile title for the currently logged in user. Does nothing if the user's email address isn't confirmed.
        /// </summary>
        /// <param name="titleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SetSelectedTitle(int titleId)
        {
            string currentUserId = User.Identity.GetUserId();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int result = await ProfileService.SetSelectedTitle(currentUserId, titleId);

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = ErrorMessages.EmailAddressNotConfirmed });
                }
            }
            catch (DbUpdateException e)
            {
                LogSelectTitleException(currentUserId, e);
                return Json(new { success = false, error = ErrorMessages.TitleNotSelected });
            }
            catch (InvalidOperationException e)
            {
                LogSelectTitleException(currentUserId, e);
                return Json(new { success = false, error = e.Message });
            }
        }

        /// <summary>
        /// Logs an exception that occurred in the SelectTitle action.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="e"></param>
        private void LogSelectTitleException(string userId, Exception e)
        {
            Log.Error(
                "SelectTitle",
                e,
                new { currentUserId = userId }
            );
        }

        #endregion Select Title

        /// <summary>
        /// Disposes of all services.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (userService != null)
                {
                    userService.Dispose();
                    userService = null;
                }

                if (violationService != null)
                {
                    violationService.Dispose();
                    violationService = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}