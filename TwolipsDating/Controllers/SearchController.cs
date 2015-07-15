using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class SearchController : BaseController
    {
        private SearchService searchService = new SearchService();

        public async Task<ActionResult> Index(string userName)
        {
            var results = await searchService.SearchProfiles(userName);

            var viewModelResults = Mapper.Map<IReadOnlyCollection<TwolipsDating.Models.Profile>, IReadOnlyCollection<ProfileViewModel>>(results);

            SearchResultViewModel viewModel = new SearchResultViewModel();
            viewModel.SearchResults = viewModelResults;

            return View(viewModel);
        }
    }
}