using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using TwolipsDating.Business;
using AutoMapper;
using TwolipsDating.ViewModels;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Configuration;
using System.Net;
using System.Data.Entity.Infrastructure;

namespace TwolipsDating.Controllers
{
    public class ProfileController : BaseController
    {
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
                    return Json(new { success = false, error = "No changes were made to your suggestion entry." });
                }
            }
            catch (DbUpdateException e)
            {
                // log e here

                return Json(new { success = false, error = "Could not update your suggestion entry." });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            string currentUserId = await GetCurrentUserIdAsync();

            await ProfileService.SendMessageAsync(currentUserId, viewModel.ProfileUserId, viewModel.SendMessage.MessageBody);

            return RedirectToIndex();
        }

        [HttpPost]
        public async Task<ActionResult> WriteReview(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            string currentUserId = await GetCurrentUserIdAsync();

            await ProfileService.WriteReviewAsync(currentUserId, viewModel.ProfileUserId, viewModel.WriteReview.ReviewContent, viewModel.WriteReview.RatingValue);

            return RedirectToIndex();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteImage(int id, string fileName, string profileUserId)
        {
            string currentUserId = await GetCurrentUserIdAsync();
            if(profileUserId != currentUserId)
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
                    return Json(new { success = false, error = "The selected image was not deleted." });
                }
            }
            catch (DbUpdateException e)
            {
                // log e here

                return Json(new { success = false, error = "Could not delete the selected image." });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage(UploadImageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex(new { tab = "pictures" });
            }

            if (viewModel.UploadedImage.ContentType != "image/jpeg"
                && viewModel.UploadedImage.ContentType != "image/png"
                && viewModel.UploadedImage.ContentType != "image/bmp"
                && viewModel.UploadedImage.ContentType != "image/gif")
            {
                return RedirectToIndex(new { tab = "pictures" });
            }

            string currentUserId = await GetCurrentUserIdAsync();

            string fileType = viewModel.UploadedImage.FileName;
            string fileName = String.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(fileType));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("twolipsdatingcdn");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            using (var stream = viewModel.UploadedImage.InputStream)
            {
                blockBlob.UploadFromStream(stream);
            }

            await ProfileService.AddUploadedImageForUserAsync(currentUserId, fileName);

            return RedirectToIndex(new { tab = "pictures" });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeImage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            string currentUserId = await GetCurrentUserIdAsync();
            if (viewModel.ProfileUserId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            await ProfileService.ChangeProfileUserImageAsync(viewModel.ProfileId, viewModel.ChangeImage.UserImageId);

            return RedirectToIndex();
        }

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
            viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "feed";
            viewModel.CurrentUserId = currentUserId;
            viewModel.AverageRatingValue = reviews.AverageRating();
            viewModel.ViewMode = ProfileViewModel.ProfileViewMode.ShowProfile;
            viewModel.SuggestedTags = tagsSuggestedForProfile; // these are the tag suggestions that will be displayed at the profile screen
            viewModel.AllTags = await GetAllTagsAndCountsInSystemAsync(tagsSuggestedForProfile); // these are all tags to be displayed in the "suggest" popup

            // setup user images and uploads
            var userImages = await ProfileService.GetUserImagesAsync(profile.ApplicationUser.Id);
            viewModel.UploadImage = new UploadImageViewModel();
            viewModel.UploadImage.CurrentUserId = currentUserId;
            viewModel.UploadImage.ProfileUserId = profile.ApplicationUser.Id;
            viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);

            // set the active tab's content
            SetViewModelBasedOnActiveTab(profile, reviews, viewModel, userImages);

            await SetUnreadCountsInViewBag();

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

        private void SetViewModelBasedOnActiveTab(Models.Profile profile, IReadOnlyCollection<Review> reviews, ProfileViewModel viewModel, IReadOnlyCollection<UserImage> userImages)
        {
            if (viewModel.ActiveTab == "feed")
            {
                viewModel.Feed = GetUserFeed(profile, reviews, userImages);
            }
            else if (viewModel.ActiveTab == "pictures")
            {
                // any picture specific stuff?
            }
            else if (viewModel.ActiveTab == "reviews")
            {
                // get the user's reviews
                viewModel.Reviews = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewViewModel>>(reviews);
            }
        }

        private IReadOnlyCollection<ProfileFeedViewModel> GetUserFeed(Models.Profile profile, IReadOnlyCollection<Review> reviews, IReadOnlyCollection<UserImage> uploadedImages)
        {
            List<ProfileFeedViewModel> viewModel = new List<ProfileFeedViewModel>();

            var reviewFeedViewModel = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewWrittenFeedViewModel>>(reviews);
            var uploadedImageFeedViewModel = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UploadedImageFeedViewModel>>(uploadedImages);

            foreach (var reviewFeed in reviewFeedViewModel)
            {
                viewModel.Add(new ProfileFeedViewModel()
                {
                    ItemType = DashboardFeedItemType.ReviewWritten,
                    DateOccurred = reviewFeed.DateOccurred,
                    ReviewWrittenFeedItem = reviewFeed
                });
            }

            foreach (var uploadedImage in uploadedImageFeedViewModel)
            {
                viewModel.Add(new ProfileFeedViewModel()
                {
                    ItemType = DashboardFeedItemType.UploadedPictures,
                    DateOccurred = uploadedImage.DateOccurred,
                    UploadedImageFeedItem = uploadedImage
                });
            }

            return viewModel.OrderByDescending(v => v.DateOccurred).ToList().AsReadOnly();
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

            viewModel.CreateProfile.Countries = countryCollection;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProfileViewModel viewModel)
        {
            string currentUserId = await GetCurrentUserIdAsync();
            DateTime birthday = new DateTime(viewModel.CreateProfile.BirthYear.Value, viewModel.CreateProfile.BirthMonth.Value, viewModel.CreateProfile.BirthDayOfMonth.Value);
            await ProfileService.CreateProfileAsync(viewModel.CreateProfile.SelectedGenderId.Value, viewModel.CreateProfile.SelectedZipCodeId, viewModel.CreateProfile.SelectedCityId.Value, currentUserId, birthday);
            return RedirectToIndex();
        }

        private ActionResult RedirectToIndex(object routeValues = null)
        {
            return RedirectToAction("index", routeValues);
        }
    }
}