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

namespace TwolipsDating.Controllers
{
    public class ProfileController : BaseController
    {
        private ProfileService profileService = new ProfileService();
        private string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        [HttpPost]
        public async Task<ActionResult> SendMessage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            var currentUser = await GetCurrentUserAsync();

            await profileService.SendMessageAsync(currentUser.Id, viewModel.ProfileUserId, viewModel.SendMessage.MessageSubject, viewModel.SendMessage.MessageBody);

            return RedirectToIndex();
        }

        [HttpPost]
        public async Task<ActionResult> WriteReview(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            var currentUser = await GetCurrentUserAsync();

            await profileService.WriteReviewAsync(currentUser.Id, viewModel.ProfileUserId, viewModel.WriteReview.ReviewContent, viewModel.WriteReview.RatingValue);

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

            var currentUser = await GetCurrentUserAsync();

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

            await profileService.AddUploadedImageForUserAsync(currentUser.Id, fileName);

            return RedirectToIndex(new { tab = "pictures" });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeImage(ProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToIndex();
            }

            await profileService.ChangeProfileUserImageAsync(viewModel.ProfileId, viewModel.ChangeImage.UserImageId);

            return RedirectToIndex();
        }

        public async Task<ActionResult> Index(string tab)
        {
            var currentUser = await GetCurrentUserAsync();
            var profile = await profileService.GetProfileAsync(currentUser.Id);

            // profile exists, let's show it
            if (profile != null)
            {
                var reviews = await profileService.GetReviewsWrittenForUserAsync(currentUser.Id);
                var viewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);
                viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "feed";
                viewModel.UploadImage = new UploadImageViewModel();

                var userImages = await profileService.GetUserImagesAsync(currentUser.Id);
                viewModel.UploadImage.UserImages = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UserImageViewModel>>(userImages);

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
                await SetUnreadCountsInViewBag(ProfileService, currentUser);

                viewModel.AverageRatingValue = reviews.AverageRating();

                return View(viewModel);
            }
            // profile doesn't exist yet, we need to ask the user for more info
            else
            {
                return await GetViewModelForProfileCreationAsync();
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
            var genders = await profileService.GetGendersAsync();
            var countries = await profileService.GetCountriesAsync();

            ProfileViewModel viewModel = new ProfileViewModel();
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
            var currentUser = await GetCurrentUserAsync();
            DateTime birthday = new DateTime(viewModel.CreateProfile.BirthYear.Value, viewModel.CreateProfile.BirthMonth.Value, viewModel.CreateProfile.BirthDayOfMonth.Value);
            await profileService.CreateProfileAsync(viewModel.CreateProfile.SelectedGenderId.Value, viewModel.CreateProfile.SelectedZipCodeId, viewModel.CreateProfile.SelectedCityId.Value, currentUser.Id, birthday);
            return RedirectToIndex();
        }

        private ActionResult RedirectToIndex(object routeValues = null)
        {
            return RedirectToAction("index", routeValues);
        }
    }
}