using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class HomeController : BaseController
    {
        #region Services

        private DashboardService dashboardService = new DashboardService();
        private ViolationService violationService = new ViolationService();

        #endregion

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = await GetCurrentUserIdAsync();
                List<DashboardItemViewModel> dashboardItems = new List<DashboardItemViewModel>();

                await AddMessagesToFeedAsync(currentUserId, dashboardItems);

                await AddReviewsToFeedAsync(dashboardItems);

                await AddUploadedImagesToFeedAsync(dashboardItems);

                await SetNotificationsAsync();

                DashboardViewModel viewModel = new DashboardViewModel();
                viewModel.CurrentUserId = currentUserId;
                viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);
                viewModel.Items = dashboardItems
                    .OrderByDescending(v => v.DateOccurred)
                    .ToList()
                    .AsReadOnly();

                var violationTypes = await violationService.GetViolationTypesAsync();
                viewModel.WriteReviewViolation = new WriteReviewViolationViewModel();
                viewModel.WriteReviewViolation.ViolationTypes = violationTypes.ToDictionary(v => v.Id, v => v.Name);

                return View("dashboard", viewModel);
            }
            else
            {
                // this is the splash page (displayed when the user isn't authenticated)
                HomeViewModel viewModel = new HomeViewModel();
                return View(String.Empty, "~/Views/Shared/_LayoutSplash.cshtml", viewModel);
            }
        }

        private async Task AddUploadedImagesToFeedAsync(List<DashboardItemViewModel> dashboardItems)
        {
            var uploadedImages = await dashboardService.GetRecentlyUploadedImagesAsync();

            var uploadedImagesConsolidated = uploadedImages.GetConsolidatedImagesForFeed();

            foreach (var userImageViewModel in uploadedImagesConsolidated)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.UploadedPictures,
                    DateOccurred = userImageViewModel.DateOccurred,
                    UploadedImageFeedItem = userImageViewModel
                });
            }
        }

        private async Task AddReviewsToFeedAsync(List<DashboardItemViewModel> dashboardItems)
        {
            var reviews = await dashboardService.GetRecentlyWrittenReviewsAsync();
            var reviewFeedViewModel = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewWrittenFeedViewModel>>(reviews);

            foreach (var reviewFeed in reviewFeedViewModel)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.ReviewWritten,
                    DateOccurred = reviewFeed.DateOccurred,
                    ReviewWrittenFeedItem = reviewFeed
                });
            }
        }

        private async Task AddMessagesToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var messages = await ProfileService.GetMessagesByUserAsync(currentUserId);
            var messageFeedViewModel = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyCollection<MessageFeedViewModel>>(messages);

            foreach (var messageFeed in messageFeedViewModel)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.Message,
                    DateOccurred = messageFeed.DateOccurred,
                    MessageFeedItem = messageFeed
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && dashboardService != null)
            {
                dashboardService.Dispose();
            }

            if (disposing && violationService != null)
            {
                violationService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}