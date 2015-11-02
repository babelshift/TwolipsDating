using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class ProfileController : BaseController
    {
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

                string profileIndexUrlRoot = Url.ActionWithFullUrl(Request, "index", "profile", new { id = (int?)null });
                bool isFavorite = await ProfileService.ToggleFavoriteProfileAsync(currentUserId, profileUserId, profileId, profileIndexUrlRoot);

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

            // if user is ignoring himself, do nothing
            if (currentUserId == profileUserId)
            {
                return Json(new { success = false, error = ErrorMessages.CannotIgnoreSelf });
            }

            var result = await ProfileService.ToggleIgnoredUserAsync(currentUserId, profileUserId);

            if (result.Succeeded)
            {
                return Json(new { success = true, isIgnored = result.ToggleStatus });
            }

            return Json(new { success = false, error = ErrorMessages.IgnoredUserNotSaved });
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
                    string profileIndexUrlRoot = Url.ActionWithFullUrl(Request, "index", "profile", new { id = (int?)null });
                    int giftCount = await ProfileService.SendGiftAsync(currentUserId, profileUserId, giftId, inventoryItemId, profileIndexUrlRoot);
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

            bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

            if (isCurrentUserEmailConfirmed)
            {
                string conversationUrl = Url.ActionWithFullUrl(Request, "conversation", "message", new { id = currentUserId });
                var result = await ProfileService.SendMessageAsync(currentUserId, profileUserId, messageBody, conversationUrl);

                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }
                else
                {
                    string errorMessage = String.Join(" ", result.Errors);
                    return Json(new { success = false, error = errorMessage });
                }
            }
            else
            {
                return Json(new { success = false, error = ErrorMessages.EmailAddressNotConfirmed });
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
                    string profileIndexUrlRoot = Url.ActionWithFullUrl(Request, "index", "profile", new { id = (int?)null });
                    int changes = await ProfileService.WriteReviewAsync(currentUserId, profileUserId, reviewContent, rating, profileIndexUrlRoot);

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

            // only allow people to delete images for their own profile
            if (profileUserId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            bool isDeleted = await DeleteImageAsync(id, fileName);

            if (isDeleted)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, error = ErrorMessages.UserImageNotDeleted });
            }
        }

        private async Task DeleteThumbnailImageFromAzureStorageAsync(string fileName)
        {
            try
            {
                var container = GetAzureStorageContainer();
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                string fileExtension = Path.GetExtension(fileName);
                string thumbnailName = String.Format("{0}_{1}{2}", fileNameWithoutExtension, "thumb", fileExtension);
                CloudBlockBlob blockBlobThumbnail = container.GetBlockBlobReference(thumbnailName);
                await blockBlobThumbnail.DeleteAsync();
            }
            catch (StorageException ex)
            {
                Log.Error("ProfileController.DeleteThumbnailImageFromAzureStorageAsync", ex, new { fileName });
            }
        }

        private async Task DeleteFullSizeImageFromAzureStorageAsync(string fileName)
        {
            try
            {
                var container = GetAzureStorageContainer();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                await blockBlob.DeleteAsync();
            }
            catch (StorageException ex)
            {
                Log.Error("ProfileController.DeleteFullSizeImageFromAzureStorageAsync", ex, new { fileName });
            }
        }

        private async Task<bool> DeleteImageAsync(int userImageId, string fileName)
        {
            var result = await ProfileService.DeleteUserImageAsync(userImageId);

            if (result.Succeeded)
            {
                await DeleteFullSizeImageFromAzureStorageAsync(fileName);
                await DeleteThumbnailImageFromAzureStorageAsync(fileName);
            }

            return result.Succeeded;
        }

        [HttpPost, RequireConfirmedEmailIfAuthenticated, ExportModelStateToTempData]
        public async Task<ActionResult> UploadImageFromModal(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = "pictures" });
            }

            await UploadImageTaskAsync(viewModel.UploadImage, replaceCurrentProfileImage: true);

            return RedirectToIndex(new { tab = "pictures" });
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
        [HttpPost, RequireConfirmedEmailIfAuthenticated, ExportModelStateToTempData]
        public async Task<ActionResult> UploadImage(UploadImageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = "pictures" });
            }

            await UploadImageTaskAsync(viewModel);

            return RedirectToIndex(new { tab = "pictures" });
        }

        private async Task UploadImageTaskAsync(UploadImageViewModel viewModel, bool replaceCurrentProfileImage = false)
        {
            string currentUserId = User.Identity.GetUserId();
            int i = 0;
            foreach (var uploadedImage in viewModel.UploadedImages)
            {
                UploadedProfileImage newImage = new UploadedProfileImage(uploadedImage);

                if (!newImage.IsValidImage) { continue; }

                // add the image id to our database
                var result = await ProfileService.AddUploadedImageForUserAsync(currentUserId, newImage.FileName);

                // if we saved to our database successfully, save to Azure Storage Blob
                if (result.Succeeded)
                {
                    // upload the full image and return it in case it had to be resized
                    await UploadFullProfileImageToAzureStorageAsync(newImage);

                    // upload the thumbnail of the now possibly resized image
                    await UploadProfileThumbnailToAzureStorageAsync(newImage);

                    // if this is the first image being uploaded and the user doesn't have a profile image set, set the user's profile image to the file being uploaded
                    if (i == 0 || replaceCurrentProfileImage)
                    {
                        var profile = await ProfileService.GetProfileAsync(currentUserId);
                        if (profile != null && (!String.IsNullOrEmpty(profile.ProfileImagePath) || replaceCurrentProfileImage))
                        {
                            await ProfileService.ChangeProfileUserImageAsync(profile.ProfileId, result.UploadedImageId);
                        }
                    }
                }
            }
        }

        private static CloudBlobContainer GetAzureStorageContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("twolipsdatingcdn");
            return container;
        }

        /// <summary>
        /// Uploads a thumbnail sized image to Azure storage.
        /// </summary>
        /// <param name="uploadedImage"></param>
        /// <param name="fileNameThumb"></param>
        /// <param name="container"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        private static async Task UploadProfileThumbnailToAzureStorageAsync(UploadedProfileImage newImage)
        {
            var container = GetAzureStorageContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(newImage.ThumbnailFileName);
            blockBlob.Properties.ContentType = newImage.ContentType;

            var rawImageThumb = newImage.ThumbnailImage.GetBytes();
            await blockBlob.UploadFromByteArrayAsync(rawImageThumb, 0, rawImageThumb.Length);
        }

        /// <summary>
        /// Uploads an image to Azure storage. Will resize the image if width or height is > 1000. This is to prevent people from uploading massively huge images and having
        /// them take up space in the database. There really isn't a reason to have huge images.
        /// </summary>
        /// <param name="uploadedImage"></param>
        /// <param name="blockBlob"></param>
        /// <returns></returns>
        private static async Task UploadFullProfileImageToAzureStorageAsync(UploadedProfileImage newImage)
        {
            var container = GetAzureStorageContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(newImage.FileName);
            blockBlob.Properties.ContentType = newImage.ContentType;

            var rawImage = newImage.FullImage.GetBytes();
            await blockBlob.UploadFromByteArrayAsync(rawImage, 0, rawImage.Length);
        }

        /// <summary>
        /// Changes a user's displayed profile image based on a selected image.
        /// Does nothing if attempting to change someone else's profile.
        /// Does nothing if the current user's email is not confirmed.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost, RequireConfirmedEmailIfAuthenticated, ExportModelStateToTempData]
        public async Task<ActionResult> ChangeImage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = viewModel.ActiveTab });
            }

            string currentUserId = User.Identity.GetUserId();

            var result = await ProfileService.ChangeProfileUserImageAsync(viewModel.ProfileId, viewModel.ChangeImage.UserImageId);

            return RedirectToIndex(new { tab = viewModel.ActiveTab });
        }

        [HttpPost, RequireConfirmedEmailIfAuthenticated]
        public async Task<JsonResult> SaveBackgroundImage(string profileUserId, int bannerPositionX, int bannerPositionY)
        {
            string currentUserId = User.Identity.GetUserId();

            var profileId = await UserService.GetProfileIdAsync(profileUserId);

            if (profileId.HasValue)
            {
                int changes = await ProfileService.SetBannerImagePositionAsync(profileId.Value, bannerPositionX, bannerPositionY);

                if (changes > 0)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        [HttpPost, RequireConfirmedEmailIfAuthenticated]
        public async Task<JsonResult> ChangeBackgroundImage(string profileUserId)
        {
            string currentUserId = User.Identity.GetUserId();

            if (Request.Files.Count > 0)
            {
                var uploadedImage = Request.Files[0] as HttpPostedFileBase;

                UploadedBannerImage newImage = new UploadedBannerImage(uploadedImage);

                if (!newImage.IsValidImage)
                {
                    return Json(new { success = false });
                }

                // add the image id to our database
                var uploadResult = await ProfileService.AddUploadedImageForUserAsync(currentUserId, newImage.FileName, true);

                // if we saved to our database successfully, save to Azure Storage Blob
                if (uploadResult.Succeeded)
                {
                    // upload the new banner to azure
                    await UploadBannerImageToAzureStorageAsync(newImage);

                    var profile = await ProfileService.GetProfileAsync(currentUserId);

                    if (profile != null)
                    {
                        if (!String.IsNullOrEmpty(profile.BannerImagePath))
                        {
                            string previousBannerFileName = profile.BannerImagePath;

                            // TODO: ATOMIC?
                            // remove the previous banner from database
                            var deleteResult = await ProfileService.DeleteBannerImageAsync(profile.BannerImageId);

                            if (deleteResult.Succeeded)
                            {
                                // remove the previous banner from azure
                                await DeleteFullSizeImageFromAzureStorageAsync(previousBannerFileName);
                            }
                        }

                        // change the user's profile to use the new banner
                        await ProfileService.ChangeProfileBannerImageAsync(profile.ProfileId, uploadResult.UploadedImageId);
                    }

                    return Json(new { success = true, bannerImagePath = UserImageExtensions.GetPath(newImage.FileName) });
                }
            }

            return Json(new { success = false });
        }

        private async Task UploadBannerImageToAzureStorageAsync(UploadedBannerImage newImage)
        {
            var container = GetAzureStorageContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(newImage.FileName);
            blockBlob.Properties.ContentType = newImage.ContentType;

            var rawImage = newImage.FullImage.GetBytes();

            await blockBlob.UploadFromByteArrayAsync(rawImage, 0, rawImage.Length);
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
        [AllowAnonymous, RequireConfirmedEmailIfAuthenticated, RequireProfileIfAuthenticated]
        public async Task<ActionResult> Index(int? id = null, string seoName = null, string tab = null, int? page = null)
        {
            string currentUserId = User.Identity.GetUserId();

            // user attempting to view profile based on explicit id
            if (id.HasValue)
            {
                var profileToBeViewed = await ProfileService.GetProfileAsync(id.Value);

                // if the user has a profile and that user isn't inactive, show the profile
                if (profileToBeViewed != null && profileToBeViewed.IsUserActive)
                {
                    string expectedSeoName = ProfileExtensions.ToSEOName(profileToBeViewed.UserName);
                    if (seoName != expectedSeoName)
                    {
                        return RedirectToAction("index", new { id = id, seoName = expectedSeoName });
                    }

                    await SetNotificationsAsync();

                    return await ShowUserProfileAsync(tab, currentUserId, profileToBeViewed, page);
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

                var currentUserProfile = await ProfileService.GetProfileAsync(currentUserId);

                await SetNotificationsAsync();

                if (currentUserProfile != null)
                {
                    return await ShowUserProfileAsync(tab, currentUserId, currentUserProfile, page);
                }
                else
                {
                    return RedirectToAction("create");
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
        private async Task<ActionResult> ShowUserProfileAsync(string tab, string currentUserId, ProfileViewModel viewModel, int? page)
        {
            // check if user's email address is confirmed
            //viewModel.IsCurrentUserEmailConfirmed = !User.Identity.IsAuthenticated ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            // set the active tab or its default
            viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "about";

            // handle review stuff
            var reviews = await ProfileService.GetReviewsWrittenForUserAsync(viewModel.ProfileUserId);
            viewModel.AverageRatingValue = reviews.AverageRating();
            viewModel.ReviewCount = reviews.Count;

            // only do certain things if the viewer of the profile is logged in
            if (User.Identity.IsAuthenticated)
            {
                // setup favorites and ignores
                viewModel.IsFavoritedByCurrentUser = await UserService.IsUserFavoritedByUserAsync(currentUserId, viewModel.ProfileUserId);
                viewModel.IsIgnoredByCurrentUser = await UserService.IsUserIgnoredByUserAsync(currentUserId, viewModel.ProfileUserId);

                // setup inventory for the viewer of the profile
                viewModel.ViewerInventoryItems = await ProfileService.GetInventoryAsync(currentUserId);

                // setup violation types
                var violationTypes = await ViolationService.GetViolationTypesAsync();
                viewModel.WriteReviewViolation = new WriteReviewViolationViewModel();
                viewModel.WriteReviewViolation.ViolationTypes = violationTypes.ToDictionary(v => v.Id, v => v.Name);

                // setup user titles to select (only if the user is viewing their own profile)
                // TODO: optimize this
                if (currentUserId == viewModel.ProfileUserId)
                {
                    var titles = await UserService.GetTitlesOwnedByUserAsync(currentUserId);
                    var titlesViewModel = titles.Select(t => new TitleViewModel()
                    {
                        TitleId = t.Key,
                        TitleName = t.Value.StoreItem.Name
                    });
                    viewModel.UserTitles = titlesViewModel.ToList().AsReadOnly();
                }

                // log a visit to the profile
                await ProfileService.LogProfileViewAsync(currentUserId, viewModel.ProfileUserId, viewModel.ProfileId);
            }

            // setup viewmodel specific to the actively selected tab
            await SetViewModelBasedOnActiveTabAsync(viewModel, reviews, currentUserId, page);

            //viewModel.FeedCount = ProfileService.GetFeedCountAsync(profile.Id);
            viewModel.PictureCount = await ProfileService.GetImagesUploadedCountByUserAsync(viewModel.ProfileUserId);
            viewModel.TagCount = await ProfileService.GetTagCountAsync(viewModel.ProfileUserId);
            viewModel.InventoryCount = await ProfileService.GetInventoryCountAsync(viewModel.ProfileUserId);
            viewModel.CompletedAchievementCount = await MilestoneService.GetCompletedAchievementCountAsync(viewModel.ProfileUserId);
            viewModel.PossibleAchievementCount = await MilestoneService.GetPossibleAchievementCountAsync();
            viewModel.SimilarUsers = await ProfileService.GetSimilarProfilesAsync(viewModel.ProfileId);

            viewModel.Followers = await ProfileService.GetFollowersAsync(viewModel.ProfileId, currentUserId);
            viewModel.Following = await ProfileService.GetFollowingAsync(viewModel.ProfileId, currentUserId);

            viewModel.FollowerCount = viewModel.Followers.Count;
            viewModel.FollowingCount = viewModel.Following.Count;

            return View(viewModel);
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
        private async Task SetViewModelBasedOnActiveTabAsync(ProfileViewModel viewModel,
            IReadOnlyCollection<Review> reviews,
            string currentUserId,
            int? page)
        {
            var lookingForTypes = await ProfileService.GetLookingForTypesAsync();
            var lookingForLocations = await ProfileService.GetLookingForLocationsAsync();
            var languages = await ProfileService.GetLanguagesAsync();
            var relationshipStatuses = await ProfileService.GetRelationshipStatusesAsync();
            viewModel.SelectedLanguages = (await ProfileService.GetSelectedLanguagesAsync(currentUserId))
                .Select(s => s.Id)
                .ToList();

            viewModel.LookingForTypes = lookingForTypes.ToDictionary(t => t.Id, t => t.Name);
            viewModel.LookingForLocations = lookingForLocations.ToDictionary(t => t.Id, t => t.Range);
            viewModel.AllLanguages = languages.ToDictionary(t => t.Id, t => t.Name);
            viewModel.RelationshipStatuses = relationshipStatuses.ToDictionary(t => t.Id, t => t.Name);

            if (viewModel.ActiveTab == "feed")
            {
                var userImages = await ProfileService.GetUserImagesAsync(viewModel.ProfileUserId);
                viewModel.UploadImage = new UploadImageViewModel();
                viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);
                viewModel.Feed = await GetUserFeedAsync(viewModel, reviews, page);
                viewModel.Feed.CurrentUserId = currentUserId;
                viewModel.Feed.ProfileUserId = viewModel.ProfileUserId;
                viewModel.Feed.ProfileUserName = viewModel.UserName;
                viewModel.Feed.ProfileId = viewModel.ProfileId;
            }
            if (viewModel.ActiveTab == "pictures" || currentUserId == viewModel.ProfileUserId)
            {
                var userImages = await ProfileService.GetUserImagesAsync(viewModel.ProfileUserId);
                viewModel.UploadImage = new UploadImageViewModel();
                viewModel.UploadImage.CurrentUserId = currentUserId;
                viewModel.UploadImage.ProfileUserId = viewModel.ProfileUserId;
                viewModel.UploadImage.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);
                viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);
            }
            if (viewModel.ActiveTab == "reviews")
            {
                viewModel.Reviews = new ProfileReviewsViewModel();
                viewModel.Reviews.Items = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewViewModel>>(reviews);
                viewModel.Reviews.CurrentUserId = currentUserId;
                viewModel.Reviews.ProfileUserId = viewModel.ProfileUserId;
                viewModel.Reviews.ProfileUserName = viewModel.UserName;
                viewModel.Reviews.ProfileId = viewModel.ProfileId;
            }
            if (viewModel.ActiveTab == "achievements")
            {
                var achievements = await MilestoneService.GetAchievementsAndStatusForUserAsync(viewModel.ProfileUserId);
                viewModel.Achievements = achievements;
            }
            if (viewModel.ActiveTab == "tags")
            {
                var tagsSuggestedForProfile = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, viewModel.ProfileId);
                viewModel.SuggestedTags = tagsSuggestedForProfile; // these are the tag suggestions that will be displayed at the profile screen
                viewModel.AllTags = await GetAllTagsAndCountsInSystemAsync(tagsSuggestedForProfile); // these are all tags to be displayed in the "suggest" popup
                viewModel.AwardedTags = await ProfileService.GetTagsAwardedToProfileAsync(viewModel.ProfileId);
            }
            if (viewModel.ActiveTab == "inventory")
            {
                viewModel.Inventory = new ProfileInventoryViewModel();
                viewModel.Inventory.Items = await ProfileService.GetInventoryAsync(viewModel.ProfileUserId);
                viewModel.Inventory.CurrentUserId = currentUserId;
                viewModel.Inventory.ProfileUserId = viewModel.ProfileUserId;
            }
            if (viewModel.ActiveTab == "stats")
            {
                var completedQuizzes = await TriviaService.GetRecentlyCompletedQuizzesByUserAsync(currentUserId);
                var completedQuizIds = completedQuizzes.Select(x => x.Id);

                viewModel.RecentlyCompletedQuizzes = await TriviaService.GetUserQuizStatsAsync(viewModel.ProfileUserId, completedQuizIds);
            }
        }

        /// <summary>
        /// Returns the profile feed view model which is used to display all feed items that have occurred for a profile including reviews, messages, and uploaded images.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="reviews"></param>
        /// <param name="uploadedImages"></param>
        /// <returns></returns>
        private async Task<ProfileFeedViewModel> GetUserFeedAsync(ProfileViewModel viewModel,
            IReadOnlyCollection<Review> reviews,
            int? page)
        {
            List<ProfileFeedItemViewModel> feedItems = new List<ProfileFeedItemViewModel>();

            AddReviewsToFeed(reviews, feedItems);

            await AddUploadedImagesToFeed(viewModel.ProfileUserId, feedItems);

            await AddGiftTransactionsToFeedAsync(viewModel.ProfileUserId, feedItems);

            await AddCompletedQuizzesToFeedAsync(viewModel.ProfileUserId, feedItems);

            await AddTagSuggestionsToFeedAsync(viewModel.ProfileUserId, feedItems);

            await AddAchievementsToFeedAsync(viewModel.ProfileUserId, feedItems);

            var orderedFeed = feedItems.OrderByDescending(v => v.DateOccurred).ToList().AsReadOnly();

            ProfileFeedViewModel viewModel2 = new ProfileFeedViewModel()
            {
                Items = orderedFeed.ToPagedList(page ?? 1, 20)
            };

            return viewModel2;
        }

        private async Task AddUploadedImagesToFeed(string userId, List<ProfileFeedItemViewModel> feedItems)
        {
            var uploadedImages = await DashboardService.GetUploadedImagesForUserFeedAsync(userId);
            var uploadedImagesConsolidated = uploadedImages.GetConsolidatedImages();

            foreach (var uploadedImage in uploadedImagesConsolidated)
            {
                feedItems.Add(new ProfileFeedItemViewModel()
                {
                    ItemType = DashboardFeedItemType.UploadedPictures,
                    DateOccurred = uploadedImage.DateOccurred,
                    UploadedImageFeedItem = uploadedImage
                });
            }
        }

        private static void AddReviewsToFeed(IReadOnlyCollection<Review> reviews, List<ProfileFeedItemViewModel> feedItems)
        {
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
        }

        private async Task AddGiftTransactionsToFeedAsync(string userId, IList<ProfileFeedItemViewModel> feedItems)
        {
            var giftTransactions = await DashboardService.GetGiftTransactionsForUserFeedAsync(userId);
            var giftTransactionsConsolidated = giftTransactions.GetConsolidatedGiftTransactions();

            foreach (var giftTransactionViewModel in giftTransactionsConsolidated)
            {
                feedItems.Add(new ProfileFeedItemViewModel()
                {
                    ItemType = DashboardFeedItemType.GiftTransaction,
                    DateOccurred = giftTransactionViewModel.DateSent,
                    GiftReceivedFeedItem = giftTransactionViewModel
                });
            }
        }

        private async Task AddCompletedQuizzesToFeedAsync(string userId, IList<ProfileFeedItemViewModel> feedItems)
        {
            var completedQuizzes = await DashboardService.GetQuizCompletionsForUserFeedAsync(userId);

            foreach (var quizCompletionViewModel in completedQuizzes)
            {
                feedItems.Add(new ProfileFeedItemViewModel()
                {
                    ItemType = DashboardFeedItemType.QuizCompletion,
                    DateOccurred = quizCompletionViewModel.DateCompleted,
                    CompletedQuizFeedItem = quizCompletionViewModel
                });
            }
        }

        private async Task AddTagSuggestionsToFeedAsync(string userId, IList<ProfileFeedItemViewModel> feedItems)
        {
            var tagsSuggested = await DashboardService.GetFollowerTagSuggestionsForUserFeedAsync(userId);
            var tagsSuggestedConsolidated = tagsSuggested.GetConsolidatedTagsSuggested();

            foreach (var tagsSuggestedViewModel in tagsSuggestedConsolidated)
            {
                feedItems.Add(new ProfileFeedItemViewModel()
                {
                    ItemType = DashboardFeedItemType.TagSuggestion,
                    DateOccurred = tagsSuggestedViewModel.DateSuggested,
                    TagSuggestionReceivedFeedItem = tagsSuggestedViewModel
                });
            }
        }

        private async Task AddAchievementsToFeedAsync(string userId, IList<ProfileFeedItemViewModel> feedItems)
        {
            var achievements = await DashboardService.GetFollowerAchievementsForUserFeedAsync(userId);

            foreach (var achievementFeedViewModel in achievements)
            {
                feedItems.Add(new ProfileFeedItemViewModel()
                {
                    ItemType = DashboardFeedItemType.AchievementObtained,
                    DateOccurred = achievementFeedViewModel.DateAchieved,
                    AchievementFeedItem = achievementFeedViewModel
                });
            }
        }

        /// <summary>
        /// If the profile doesn't exist when the user wants to view their own profile, use a special view model which only allows for the creation of a profile.
        /// </summary>
        /// <returns></returns>
        private async Task<CreateProfileViewModel> GetViewModelForProfileCreationAsync()
        {
            var genders = await ProfileService.GetGendersAsync();

            CreateProfileViewModel viewModel = new CreateProfileViewModel();

            Dictionary<int, string> genderCollection = new Dictionary<int, string>();
            foreach (var gender in genders)
            {
                genderCollection.Add(gender.Id, gender.Name);
            }

            viewModel.Genders = genderCollection;

            string currentUserId = User.Identity.GetUserId();
            viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

            viewModel.Months = CalendarHelper.GetMonths().ToDictionary(m => m.MonthNumber, m => m.MonthName);
            viewModel.Years = CalendarHelper.GetYears().ToDictionary(m => m, m => m);
            viewModel.Days = CalendarHelper.GetDaysOfMonth(Months.January).ToDictionary(m => m, m => m);

            return viewModel;
        }

        #endregion Index and Show Profile

        #region Create Profile

        [ImportModelStateFromTempData]
        public async Task<ActionResult> Create()
        {
            CreateProfileViewModel viewModel = await GetViewModelForProfileCreationAsync();
            return View(viewModel);
        }

        [HttpPost, RequireValidCaptcha, ExportModelStateToTempData]
        public async Task<ActionResult> Create(CreateProfileViewModel viewModel, bool isCaptchaValid)
        {
            if (!ModelState.IsValid || !isCaptchaValid)
            {
                if (!isCaptchaValid)
                {
                    ModelState.AddModelError(Guid.NewGuid().ToString(), "You need to solve the captcha.");
                }

                int selectedGenderId = viewModel.SelectedGenderId ?? default(int);
                int day = viewModel.BirthDayOfMonth ?? default(int);
                int month = viewModel.BirthMonth ?? default(int);
                int year = viewModel.BirthYear ?? default(int);
                viewModel = await GetViewModelForProfileCreationAsync();
                viewModel.SelectedGenderId = selectedGenderId;
                viewModel.BirthDayOfMonth = day;
                viewModel.BirthMonth = month;
                viewModel.BirthYear = year;
                return View(viewModel);
            }

            string currentUserId = User.Identity.GetUserId();
            DateTime birthday = new DateTime(viewModel.BirthYear.Value, viewModel.BirthMonth.Value, viewModel.BirthDayOfMonth.Value);

            string[] location = viewModel.SelectedLocation.Split(',');
            string cityName = location[0].Trim();
            string stateAbbreviation = location[1].Trim();
            string countryName = location[2].Trim();

            var result = await ProfileService.CreateProfileAsync(
                viewModel.SelectedGenderId.Value,
                cityName,
                stateAbbreviation,
                countryName,
                currentUserId, birthday);

            if (result.Succeeded)
            {
                return RedirectToIndex();
            }
            else
            {
                return RedirectToAction("create");
            }
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

        #region Achievements

        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
        public async Task<ActionResult> Achievements()
        {
            await SetNotificationsAsync();

            string currentUserId = User.Identity.GetUserId();

            var achievements = await MilestoneService.GetAchievementsAndStatusForUserAsync(currentUserId);

            AchievementManagerViewModel viewModel = new AchievementManagerViewModel();
            viewModel.Achievements = achievements;

            return View(viewModel);
        }

        #endregion Achievements

        #region Self Summary Stuff

        [HttpPost]
        public async Task<JsonResult> SaveSelfSummary(string selfSummary)
        {
            string currentUserId = User.Identity.GetUserId();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int result = await ProfileService.SetSelfSummaryAsync(currentUserId, selfSummary);

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
                return Json(new { success = false, error = ErrorMessages.SelfSummaryNotSaved });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveSummaryOfDoing(string summaryOfDoing)
        {
            string currentUserId = User.Identity.GetUserId();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int result = await ProfileService.SetSummaryOfDoingAsync(currentUserId, summaryOfDoing);

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
                return Json(new { success = false, error = ErrorMessages.SummaryOfDoingNotSaved });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveSummaryOfGoing(string summaryOfGoing)
        {
            string currentUserId = User.Identity.GetUserId();

            try
            {
                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int result = await ProfileService.SetSummaryOfGoingAsync(currentUserId, summaryOfGoing);

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
                return Json(new { success = false, error = ErrorMessages.SummaryOfGoingNotSaved });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveLookingFor(ProfileViewModel viewModel)
        {
            string currentUserId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
            }

            int changes = await ProfileService.SetLookingForAsync(currentUserId, viewModel.LookingForTypeId, viewModel.LookingForLocationId, viewModel.LookingForAgeMin, viewModel.LookingForAgeMax);

            return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
        }

        [HttpPost]
        public async Task<ActionResult> SaveMyDetails(ProfileViewModel viewModel)
        {
            string currentUserId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
            }

            IReadOnlyCollection<int> languageIds = new ReadOnlyCollection<int>(viewModel.SelectedLanguages);
            int changes = await ProfileService.SetDetailsAsync(currentUserId, languageIds, viewModel.RelationshipStatusId);

            return RedirectToIndex(new { id = viewModel.ProfileId, tab = viewModel.ActiveTab });
        }

        #endregion Self Summary Stuff

        #region Quick Match

        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
        public async Task<ActionResult> Quick()
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            var viewModel = await ProfileService.GetRandomProfileAsync(currentUserId);

            var userImages = await ProfileService.GetUserImagesAsync(viewModel.ProfileUserId);
            viewModel.UploadImage = new UploadImageViewModel();
            viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);

            viewModel.IsFavoritedByCurrentUser = await ProfileService.IsProfileFavoritedByUserAsync(viewModel.ProfileId, currentUserId);

            return View(viewModel);
        }

        #endregion

        public ActionResult AchievementShowcase(string userId)
        {
            AchievementShowcaseViewModel viewModel = new AchievementShowcaseViewModel();

            viewModel.ProfileUserId = userId;
            viewModel.Items = ProfileService.GetAchievementShowcaseItems(userId);

            return PartialView("_AchievementShowcasePartial", viewModel);
        }

        public async Task<JsonResult> GetCompletedAchievements(string profileUserId)
        {
            var completedAchievements = await MilestoneService.GetCompletedAchievementsAsync(profileUserId);

            return Json(new { result = completedAchievements }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SetAchievementOnShowcase(int newMilestoneId, int? currentMilestoneId)
        {
            string currentUserId = User.Identity.GetUserId();

            var result = await ProfileService.SetAchievementOnShowcaseAsync(currentUserId, newMilestoneId, currentMilestoneId);

            if(result.Succeeded)
            {
                return Json(
                    new
                    {
                        success = true,
                        newAchievementImagePath = result.NewAchievementOnShowcase.AchievementImagePath,
                        newAchievementName = result.NewAchievementOnShowcase.AchievementName
                    }
                );
            }
            else
            {
                return Json(new { success = false, errors = result.Errors });
            }
        }
    }
}