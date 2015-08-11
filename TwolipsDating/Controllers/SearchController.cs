using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class SearchController : BaseController
    {
        #region Services

        private SearchService searchService = new SearchService();

        #endregion Services

        /// <summary>
        /// Sets up a view model which can be used to display search results based on a user name or a tag name.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> Index(string user)
        {
            string currentUserId = User.Identity.GetUserId();

            await SetNotificationsAsync();

            SearchResultViewModel viewModel = new SearchResultViewModel();

            if (!String.IsNullOrEmpty(user))
            {
                var results = await searchService.SearchProfilesByUserName(user);

                viewModel.SearchResults = Mapper.Map<IReadOnlyCollection<TwolipsDating.Models.Profile>, IReadOnlyCollection<ProfileViewModel>>(results);

                // TODO: optimize this by eager loading?
                foreach (var profileViewModel in viewModel.SearchResults)
                {
                    var reviews = await ProfileService.GetReviewsWrittenForUserAsync(profileViewModel.ProfileUserId);
                    profileViewModel.AverageRatingValue = reviews.AverageRating();
                    profileViewModel.ReviewCount = reviews.Count;

                    // tag suggestions and awards
                    // this will break for anonymous users
                    profileViewModel.SuggestedTags = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, profileViewModel.ProfileId);
                }

                viewModel.User = user;
            }

            return View(viewModel);
        }

        public async Task<ActionResult> Tag(string tag)
        {
            string currentUserId = User.Identity.GetUserId();

            await SetNotificationsAsync();

            SearchResultViewModel viewModel = new SearchResultViewModel();

            if (!String.IsNullOrEmpty(tag))
            {
                var results = await searchService.SearchProfilesByTagName(tag);

                viewModel.SearchResults = Mapper.Map<IReadOnlyCollection<TwolipsDating.Models.Profile>, IReadOnlyCollection<ProfileViewModel>>(results);

                // TODO: optimize this by eager loading?
                foreach (var profileViewModel in viewModel.SearchResults)
                {
                    var reviews = await ProfileService.GetReviewsWrittenForUserAsync(profileViewModel.ProfileUserId);
                    profileViewModel.AverageRatingValue = reviews.AverageRating();
                    profileViewModel.ReviewCount = reviews.Count;

                    // tag suggestions and awards
                    // this will break for anonymous users
                    profileViewModel.SuggestedTags = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, profileViewModel.ProfileId);
                }

                viewModel.User = tag;
            }

            return View(viewModel);
        }

        /// <summary>
        /// Disposes all services.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && searchService != null)
            {
                searchService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}