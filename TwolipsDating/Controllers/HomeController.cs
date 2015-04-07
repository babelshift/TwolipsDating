using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class HomeController : BaseController
    {
        private DashboardService dashboardService = new DashboardService();

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // dashboard
                string currentUserId = await GetCurrentUserIdAsync();
                List<DashboardItemViewModel> dashboardItems = new List<DashboardItemViewModel>();

                var messages = await ProfileService.GetMessagesByUserAsync(currentUserId);
                var messageFeedViewModel = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyCollection<MessageFeedViewModel>>(messages);

                var reviews = await dashboardService.GetRecentlyWrittenReviewsAsync();
                var reviewFeedViewModel = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewWrittenFeedViewModel>>(reviews);

                foreach(var messageFeed in messageFeedViewModel)
                {
                    dashboardItems.Add(new DashboardItemViewModel()
                    {
                        ItemType = DashboardFeedItemType.Message,
                        DateOccurred = messageFeed.DateOccurred,
                        MessageFeedItem = messageFeed
                    });
                }

                foreach (var reviewFeed in reviewFeedViewModel)
                {
                    dashboardItems.Add(new DashboardItemViewModel()
                    {
                        ItemType = DashboardFeedItemType.ReviewWritten,
                        DateOccurred = reviewFeed.DateOccurred,
                        ReviewWrittenFeedItem = reviewFeed
                    });
                }

                var uploadedImages = await dashboardService.GetRecentlyUploadedImagesAsync();

                var uploadedImagesConsolidated = uploadedImages.GetConsolidatedImagesForFeed();

                foreach(var userImageViewModel in uploadedImagesConsolidated)
                {
                    dashboardItems.Add(new DashboardItemViewModel()
                    {
                        ItemType = DashboardFeedItemType.UploadedPictures,
                        DateOccurred = userImageViewModel.DateOccurred,
                        UploadedImageFeedItem = userImageViewModel
                    });
                }

                await SetUnreadCountsInViewBag();
                
                DashboardViewModel viewModel = new DashboardViewModel();
                viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);
                viewModel.Items = dashboardItems.OrderByDescending(v => v.DateOccurred).ToList().AsReadOnly();

                return View("dashboard", viewModel);
            }
            else
            {
                // splash
                HomeViewModel viewModel = new HomeViewModel();
                return View(String.Empty, "~/Views/Shared/_LayoutSplash.cshtml", viewModel);
            }
        }
    }
}