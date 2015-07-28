using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.ViewModels;
using TwolipsDating.Utilities;
using Microsoft.AspNet.Identity;

namespace TwolipsDating.Controllers
{
    public class SearchController : BaseController
    {
        private SearchService searchService = new SearchService();

        public async Task<ActionResult> Index(string user, string tag)
        {
            string currentUserId = User.Identity.GetUserId();
            await SetNotificationsAsync();

            SearchResultViewModel viewModel = new SearchResultViewModel();

            if(!String.IsNullOrEmpty(user) || !String.IsNullOrEmpty(tag))
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
                    profileViewModel.SuggestedTags = await ProfileService.GetTagsSuggestedForProfileAsync(currentUserId, profileViewModel.ProfileId);
                }

                viewModel.User = user;
                viewModel.Tag = tag;
            }

            return View(viewModel);
        }

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