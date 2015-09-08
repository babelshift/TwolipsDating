using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;
using PagedList;

namespace TwolipsDating.Controllers
{
    public class HomeController : BaseController
    {
        #region Services

        private DashboardService dashboardService = new DashboardService();
        private ViolationService violationService = new ViolationService();
        private TriviaService triviaService;
        private UserService userService = new UserService();

        #endregion Services

        public HomeController()
        {
            triviaService = new TriviaService(new ModelStateWrapper(ModelState));
        }

        [RequireProfileIfAuthenticated]
        [AllowAnonymous]
        public async Task<ActionResult> Index(int? page)
        {
            // if the user is authenticated, allow them to view their dashboard
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.Identity.GetUserId();

                List<DashboardItemViewModel> dashboardItems = new List<DashboardItemViewModel>();

                await AddMessagesToFeedAsync(currentUserId, dashboardItems);

                await AddReviewsToFeedAsync(currentUserId, dashboardItems);

                await AddUploadedImagesToFeedAsync(currentUserId, dashboardItems);

                await AddGiftTransactionsToFeedAsync(currentUserId, dashboardItems);

                await AddCompletedQuizzesToFeedAsync(currentUserId, dashboardItems);

                await AddTagSuggestionsToFeedAsync(currentUserId, dashboardItems);

                await AddAchievementsToFeedAsync(currentUserId, dashboardItems);

                await AddFollowersToFeedAsync(currentUserId, dashboardItems);

                await SetNotificationsAsync();

                DashboardViewModel viewModel = new DashboardViewModel();
                viewModel.CurrentUserId = currentUserId;
                viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                int pageSize = 20;
                int pageNumber = page ?? 1;

                viewModel.Items = dashboardItems
                    .OrderByDescending(v => v.DateOccurred)
                    .ToList()
                    .AsReadOnly()
                    .ToPagedList(pageNumber, pageSize);

                await SetupReviewViolationsOnDashboardAsync(viewModel);

                await SetupRandomQuestionOnDashboardAsync(currentUserId, viewModel);

                await SetupQuizzesOnDashboardAsync(currentUserId, viewModel);

                await SetupUsersToFollowAsync(currentUserId, viewModel);

                return View("dashboard", viewModel);
            }
            // if the user isn't authenticated, show them the new user splash page
            else
            {
                HomeViewModel viewModel = new HomeViewModel();
                return View(String.Empty, "~/Views/Shared/_LayoutSplash.cshtml", viewModel);
            }
        }

        private async Task SetupUsersToFollowAsync(string currentUserId, DashboardViewModel viewModel)
        {
            int profilesToRetrieve = 8;
            var profiles = await ProfileService.GetRandomProfilesForDashboardAsync(currentUserId, profilesToRetrieve);
            viewModel.UsersToFollow = Mapper.Map<IReadOnlyCollection<Models.Profile>, IReadOnlyCollection<ProfileViewModel>>(profiles);
        }

        /// <summary>
        /// Adds the necessary components to the view model to allow a user to select review violation options
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetupReviewViolationsOnDashboardAsync(DashboardViewModel viewModel)
        {
            var violationTypes = await violationService.GetViolationTypesAsync();
            viewModel.WriteReviewViolation = new WriteReviewViolationViewModel();
            viewModel.WriteReviewViolation.ViolationTypes = violationTypes.ToDictionary(v => v.Id, v => v.Name);
        }

        /// <summary>
        /// Sets up the view model to contain a list of quizzes and whether or not the current user has completed those quizzes
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetupQuizzesOnDashboardAsync(string currentUserId, DashboardViewModel viewModel)
        {
            var quizzes = await triviaService.GetQuizzesAsync();
            viewModel.Quizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(quizzes);
            var completedQuizzes = await triviaService.GetCompletedQuizzesForUserAsync(currentUserId);

            foreach (var quiz in viewModel.Quizzes)
            {
                if (completedQuizzes.Any(q => q.Key == quiz.Id))
                {
                    quiz.IsComplete = true;
                }
            }
        }

        /// <summary>
        /// Sets up the view model to contain a random question to display to a user
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetupRandomQuestionOnDashboardAsync(string currentUserId, DashboardViewModel viewModel)
        {
            // generate a random question with its answers to view
            var randomQuestion = await triviaService.GetRandomQuestionAsync(currentUserId, (int)QuestionTypeValues.Random);
            viewModel.RandomQuestion = Mapper.Map<Question, QuestionViewModel>(randomQuestion);
        }

        /// <summary>
        /// Queries the database for recent images uploaded by the current user's followers and adds them to the view model.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="dashboardItems"></param>
        /// <returns></returns>
        private async Task AddUploadedImagesToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var uploadedImages = await dashboardService.GetRecentFollowerImagesAsync(currentUserId);
            var uploadedImagesConsolidated = uploadedImages.GetConsolidatedImages();

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

        /// <summary>
        /// Queries the database for recent reviews written by the current user's followers and adds them to the view model.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="dashboardItems"></param>
        /// <returns></returns>
        private async Task AddReviewsToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var reviews = await dashboardService.GetRecentFollowerReviewsAsync(currentUserId);
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

        /// <summary>
        /// Queries the database for recent messages written by the current user's followers to the current user and adds them to the view model.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="dashboardItems"></param>
        /// <returns></returns>
        private async Task AddMessagesToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var messages = await ProfileService.GetMessagesReceivedByUserAsync(currentUserId);
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

        private async Task AddGiftTransactionsToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var giftTransactions = await dashboardService.GetRecentFollowerGiftTransactionsAsync(currentUserId);
            var giftTransactionsConsolidated = giftTransactions.GetConsolidatedGiftTransactions();

            foreach (var giftTransactionViewModel in giftTransactionsConsolidated)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.GiftTransaction,
                    DateOccurred = giftTransactionViewModel.DateSent,
                    GiftReceivedFeedItem = giftTransactionViewModel
                });
            }
        }

        private async Task AddCompletedQuizzesToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var completedQuizzes = await dashboardService.GetRecentFollowerQuizCompletionsAsync(currentUserId);

            foreach (var quizCompletionViewModel in completedQuizzes)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.QuizCompletion,
                    DateOccurred = quizCompletionViewModel.DateCompleted,
                    CompletedQuizFeedItem = quizCompletionViewModel
                });
            }
        }

        private async Task AddTagSuggestionsToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var tagsSuggested = await dashboardService.GetRecentFollowerTagSuggestionsAsync(currentUserId);
            var tagsSuggestedConsolidated = tagsSuggested.GetConsolidatedTagsSuggested();

            foreach (var tagsSuggestedViewModel in tagsSuggestedConsolidated)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.TagSuggestion,
                    DateOccurred = tagsSuggestedViewModel.DateSuggested,
                    TagSuggestionReceivedFeedItem = tagsSuggestedViewModel
                });
            }
        }

        private async Task AddAchievementsToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var achievements = await dashboardService.GetRecentFollowerAchievementsAsync(currentUserId);
            var achievementFeedViewModels = Mapper.Map<IReadOnlyCollection<MilestoneAchievement>, IReadOnlyCollection<AchievementFeedViewModel>>(achievements);

            foreach (var achievementFeedViewModel in achievementFeedViewModels)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.AchievementObtained,
                    DateOccurred = achievementFeedViewModel.DateAchieved,
                    AchievementFeedItem = achievementFeedViewModel
                });
            }
        }

        private async Task AddFollowersToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var followers = await dashboardService.GetRecentFollowersAsync(currentUserId);
            var followerFeedViewModels = Mapper.Map<IReadOnlyCollection<FavoriteProfile>, IReadOnlyCollection<FollowerFeedViewModel>>(followers);

            foreach (var followerFeedViewModel in followerFeedViewModels)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.NewFollower,
                    DateOccurred = followerFeedViewModel.DateFollowed,
                    FollowerFeedItem = followerFeedViewModel
                });
            }
        }

        /// <summary>
        /// Disposes of the services and their associated DbContexts.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dashboardService != null)
                {
                    dashboardService.Dispose();
                    dashboardService = null;
                }

                if (violationService != null)
                {
                    violationService.Dispose();
                    violationService = null;
                }

                if (triviaService != null)
                {
                    triviaService.Dispose();
                    triviaService = null;
                }

                if (userService != null)
                {
                    userService.Dispose();
                    userService = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}