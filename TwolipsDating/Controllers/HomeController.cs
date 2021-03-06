﻿using AutoMapper;
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
        #region Dashboard

        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
        public async Task<ActionResult> Index(int? page)
        {
            // if the user is authenticated, allow them to view their dashboard
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.Identity.GetUserId();

                List<DashboardItemViewModel> dashboardItems = new List<DashboardItemViewModel>();

                await AddReceivedMessagesToFeedAsync(currentUserId, dashboardItems);

                await AddReviewsToFeedAsync(currentUserId, dashboardItems, FeedItemQueryType.All);

                await AddUploadedImagesToFeedAsync(currentUserId, dashboardItems);

                await AddGiftTransactionsToFeedAsync(currentUserId, dashboardItems, FeedItemQueryType.All);

                await AddCompletedQuizzesToFeedAsync(currentUserId, dashboardItems);

                await AddTagSuggestionsToFeedAsync(currentUserId, dashboardItems);

                await AddAchievementsToFeedAsync(currentUserId, dashboardItems);

                await AddFollowersToFeedAsync(currentUserId, dashboardItems);

                await SetNotificationsAsync();

                DashboardViewModel viewModel = new DashboardViewModel();
                viewModel.CurrentUserId = currentUserId;

                int pageSize = 20;
                int pageNumber = page ?? 1;

                viewModel.Items = dashboardItems
                    .OrderByDescending(v => v.DateOccurred)
                    .ToList()
                    .ToPagedList(pageNumber, pageSize);

                await SetupReviewViolationsOnDashboardAsync(viewModel);

                await SetupRandomQuestionOnDashboardAsync(currentUserId, viewModel);

                //await SetupQuizzesOnDashboardAsync(currentUserId, viewModel);

                viewModel.UsersToFollow = await GetUsersToFollowAsync(currentUserId);

                return View("dashboard", viewModel);
            }
            // if the user isn't authenticated, show them the new user splash page
            else
            {
                HomeViewModel viewModel = new HomeViewModel();
                //Random r = new Random();
                //int i = r.Next(1, 10);
                //string cdn = ConfigurationManager.AppSettings["cdnUrl"];
                //viewModel.BackgroundImage = String.Format("{0}/LandingPage{1}.jpg", cdn, i);
                return View(String.Empty, "~/Views/Shared/_LayoutSplash.cshtml", viewModel);
            }
        }

        #endregion

        #region Notifications

        [RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
        public async Task<ActionResult> Notifications(int? page)
        {
            await ClearNotificationsAsync();
            await SetNotificationsAsync();

            string currentUserId = User.Identity.GetUserId();

            NotificationsViewModel viewModel = new NotificationsViewModel();
            viewModel.UsersToFollow = await GetUsersToFollowAsync(currentUserId);

            List<DashboardItemViewModel> notificationItems = new List<DashboardItemViewModel>();
            await AddReceivedMessagesToFeedAsync(currentUserId, notificationItems);
            await AddReviewsToFeedAsync(currentUserId, notificationItems, FeedItemQueryType.Self);
            await AddGiftTransactionsToFeedAsync(currentUserId, notificationItems, FeedItemQueryType.Self);
            await AddFollowersToFeedAsync(currentUserId, notificationItems);
            await AddProfileVisitsToFeedAsync(currentUserId, notificationItems);

            int pageSize = 20;
            int pageNumber = page ?? 1;

            viewModel.Items = notificationItems
                .OrderByDescending(v => v.DateOccurred)
                .ToList()
                .ToPagedList(pageNumber, pageSize);

            return View(viewModel);
        }

        #endregion

        private async Task<IReadOnlyCollection<PersonYouMightAlsoLikeViewModel>> GetUsersToFollowAsync(string currentUserId)
        {
            int profilesToRetrieve = 8;
            var profiles = await ProfileService.GetRandomProfilesForDashboardAsync(currentUserId, profilesToRetrieve);
            return profiles;
        }

        /// <summary>
        /// Adds the necessary components to the view model to allow a user to select review violation options
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetupReviewViolationsOnDashboardAsync(DashboardViewModel viewModel)
        {
            var violationTypes = await ViolationService.GetViolationTypesAsync();
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
            var quizzes = await TriviaService.GetQuizzesAsync();
            viewModel.Quizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(quizzes);
            var completedQuizzes = await TriviaService.GetCompletedQuizzesByUserAsync(currentUserId);

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
            var randomQuestion = await TriviaService.GetRandomQuestionAsync(currentUserId, (int)QuestionTypeValues.Random);
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
            var uploadedImages = await DashboardService.GetFollowerUploadedImagesAsync(currentUserId);
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
        private async Task AddReviewsToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems, FeedItemQueryType queryType)
        {
            var reviews = await DashboardService.GetReviewsForFeedAsync(currentUserId, queryType);

            foreach (var reviewFeed in reviews)
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
        private async Task AddReceivedMessagesToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems)
        {
            var messages = await ProfileService.GetMessagesReceivedByUserFeedAsync(currentUserId);

            foreach (var messageFeed in messages)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.Message,
                    DateOccurred = messageFeed.DateOccurred,
                    MessageFeedItem = messageFeed
                });
            }
        }

        private async Task AddGiftTransactionsToFeedAsync(string currentUserId, List<DashboardItemViewModel> dashboardItems, FeedItemQueryType queryType)
        {
            var giftTransactions = await DashboardService.GetFollowerGiftTransactionsAsync(currentUserId, queryType);
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
            var completedQuizzes = await DashboardService.GetFollowerQuizCompletionsAsync(currentUserId);

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
            var tagsSuggested = await DashboardService.GetFollowerTagSuggestionsAsync(currentUserId);
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
            var achievements = await DashboardService.GetFollowerAchievementsAsync(currentUserId);

            foreach (var achievementFeedViewModel in achievements)
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
            var followers = await DashboardService.GetFollowersAsync(currentUserId);

            foreach (var followerFeedViewModel in followers)
            {
                dashboardItems.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.NewFollower,
                    DateOccurred = followerFeedViewModel.DateFollowed,
                    FollowerFeedItem = followerFeedViewModel
                });
            }
        }

        private async Task AddProfileVisitsToFeedAsync(string currentUserId, List<DashboardItemViewModel> items)
        {
            var visitors = await DashboardService.GetProfileVisitsAsync(currentUserId);

            foreach (var visitorFeedViewModel in visitors)
            {
                items.Add(new DashboardItemViewModel()
                {
                    ItemType = DashboardFeedItemType.ProfileVisit,
                    DateOccurred = visitorFeedViewModel.DateOccurred,
                    ProfileVisitFeedItem = visitorFeedViewModel
                });
            }
        }
    }
}