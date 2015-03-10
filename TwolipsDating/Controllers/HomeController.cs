using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class HomeController : BaseController
    {
        private ProfileService profileService = new ProfileService();
        private DashboardService dashboardService = new DashboardService();

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // dashboard
                var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
                List<DashboardViewModel> viewModel = new List<DashboardViewModel>();

                var messages = await profileService.GetMessagesByUserAsync(currentUser.Id);
                var messageFeedViewModel = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyCollection<MessageFeedViewModel>>(messages);

                var reviews = await dashboardService.GetRecentlyWrittenReviewsAsync();
                var reviewFeedViewModel = Mapper.Map<IReadOnlyCollection<Review>, IReadOnlyCollection<ReviewWrittenFeedViewModel>>(reviews);

                var uploadedImages = await dashboardService.GetRecentlyUploadedImagesAsync();
                var uploadedImageFeedViewModel = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UploadedImageFeedViewModel>>(uploadedImages);

                foreach(var messageFeed in messageFeedViewModel)
                {
                    viewModel.Add(new DashboardViewModel()
                    {
                        ItemType = DashboardFeedItemType.Message,
                        DateOccurred = messageFeed.DateOccurred,
                        MessageFeedItem = messageFeed
                    });
                }

                foreach (var reviewFeed in reviewFeedViewModel)
                {
                    viewModel.Add(new DashboardViewModel()
                    {
                        ItemType = DashboardFeedItemType.ReviewWritten,
                        DateOccurred = reviewFeed.DateOccurred,
                        ReviewWrittenFeedItem = reviewFeed
                    });
                }

                foreach (var uploadedImage in uploadedImageFeedViewModel)
                {
                    viewModel.Add(new DashboardViewModel()
                    {
                        ItemType = DashboardFeedItemType.UploadedPictures,
                        DateOccurred = uploadedImage.DateOccurred,
                        UploadedImageFeedItem = uploadedImage
                    });
                }

                await SetUnreadCountsInViewBag();

                return View("dashboard", viewModel.OrderByDescending(v => v.DateOccurred).ToList().AsReadOnly());
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