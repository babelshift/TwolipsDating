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
        public async Task<ActionResult> Index(string user, string tag)
        {
            string currentUserId = User.Identity.GetUserId();

            await SetNotificationsAsync();

            SearchResultViewModel viewModel = new SearchResultViewModel();

            if (!String.IsNullOrEmpty(user) || !String.IsNullOrEmpty(tag))
            {
                var results = await GetSearchResults(user, tag);

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
                viewModel.Tag = tag;
            }

            return View(viewModel);
        }

        /// <summary>
        /// Returns a collection of profiles that match either the user name or the tag or both.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<Models.Profile>> GetSearchResults(string userName, string tag)
        {
            IReadOnlyCollection<Models.Profile> results = null;

            // tag only provided
            if (String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(tag))
            {
                results = await searchService.SearchProfilesByTagName(tag);
            }
            // username only provided
            else if (!String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(tag))
            {
                results = await searchService.SearchProfilesByUserName(userName);
            }
            // both provided
            else if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(tag))
            {
                results = await searchService.SearchProfilesByUserNameAndTagName(userName, tag);
            }

            return results;
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