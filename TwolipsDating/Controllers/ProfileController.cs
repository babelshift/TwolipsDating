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

namespace TwolipsDating.Controllers
{
    public class ProfileController : BaseController
    {
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

            await ProfileService.ChangeProfileUserImageAsync(viewModel.ProfileId, viewModel.ChangeImage.UserImageId);

            return RedirectToIndex();
        }

        public async Task<ActionResult> Index(int? id, string tab)
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
            var reviews = await ProfileService.GetReviewsWrittenForUserAsync(profile.ApplicationUser.Id);
            var viewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);
            viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "feed";
            viewModel.CurrentUserId = currentUserId;
            viewModel.AverageRatingValue = reviews.AverageRating();
            viewModel.ViewMode = ProfileViewModel.ProfileViewMode.ShowProfile;

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