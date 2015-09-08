using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;
using PagedList;

namespace TwolipsDating.Controllers
{
    public class SearchController : BaseController
    {
        #region Services

        private SearchService searchService;

        #endregion Services

        public SearchController()
        {
            searchService = new SearchService(new ModelStateWrapper(ModelState));
        }

        /// <summary>
        /// Sets up a view model which can be used to display search results based on a user name or a tag name.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> Index(string[] tags, int? page)
        {
            string currentUserId = User.Identity.GetUserId();

            await SetNotificationsAsync();

            SearchResultViewModel viewModel = new SearchResultViewModel();

            viewModel.SearchTags = await GetSearchableTags();

            if (tags != null && tags.Length > 0)
            {
                var results = await searchService.GetProfilesByTagNamesAsync(tags);

                await SetupViewModel(tags, currentUserId, viewModel, results, page);
            }

            return View(viewModel);
        }

        private async Task<Dictionary<string, string>> GetSearchableTags()
        {
            var allTags = await ProfileService.GetAllTagsAsync();
            Dictionary<string, string> d = new Dictionary<string, string>();

            foreach (var tag in allTags)
            {
                d.Add(tag.Name, tag.Name);
            }

            return d;
        }

        private async Task SetupViewModel(string[] tags, string currentUserId, SearchResultViewModel viewModel, IReadOnlyCollection<Models.Profile> results, int? page)
        {
            var searchResults = Mapper.Map<IReadOnlyCollection<TwolipsDating.Models.Profile>, IReadOnlyCollection<ProfileViewModel>>(results);

            // TODO: optimize this by eager loading?
            foreach (var profileViewModel in searchResults)
            {
                var reviews = await ProfileService.GetReviewsWrittenForUserAsync(profileViewModel.ProfileUserId);
                profileViewModel.AverageRatingValue = reviews.AverageRating();
                profileViewModel.ReviewCount = reviews.Count;

                // tag suggestions and awards
                // this will break for anonymous users
                profileViewModel.SuggestedTags = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, profileViewModel.ProfileId);
            }

            viewModel.SearchResults = searchResults.ToPagedList(page ?? 1, 20);

            viewModel.Tags = new List<string>();
            foreach(var tag in tags)
            {
                viewModel.Tags.Add(tag);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> Quiz(string tag)
        {
            string currentUserId = User.Identity.GetUserId();

            await SetNotificationsAsync();

            var quizzes = await searchService.GetQuizzesByTagsAsync(tag);

            return View(quizzes);
        }

        /// <summary>
        /// Disposes all services.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (searchService != null)
                {
                    searchService.Dispose();
                    searchService = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}