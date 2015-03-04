using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using TwolipsDating.Business;
using AutoMapper;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class ProfileController : BaseController
    {
        private ProfileService profileService = new ProfileService();

        public async Task<ActionResult> SendMessage(ProfileViewModel viewModel)
        {
            var currentUser = await GetCurrentUserAsync();

            await profileService.SendMessageAsync(currentUser.Id, viewModel.ProfileUserId, viewModel.MessageSubject, viewModel.MessageBody);
            
            return RedirectToIndex();
        }

        public async Task<ActionResult> WriteReview(ProfileViewModel viewModel)
        {
            var currentUser = await GetCurrentUserAsync();

            await profileService.WriteReviewAsync(currentUser.Id, viewModel.ProfileUserId, viewModel.ReviewContent, viewModel.RatingValue);

            return RedirectToIndex();
        }

        public async Task<ActionResult> Index(string tab)
        {
            var currentUser = await GetCurrentUserAsync();
            var profile = await profileService.GetProfileAsync(currentUser.Id);

            // profile exists, let's show it
            if (profile != null)
            {
                var viewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);
                if(tab == "feed")
                {

                }
                else if(tab == "pictures")
                {

                }
                else if(tab == "reviews")
                {

                }
                SetUnreadCountsInViewBag(ProfileService, currentUser);

                viewModel.ActiveTab = !String.IsNullOrEmpty(tab) ? tab : "feed";

                return View(viewModel);
            }
            // profile doesn't exist yet, we need to ask the user for more info
            else
            {
                return GetViewModelForProfileCreation();
            }
        }

        private ActionResult GetViewModelForProfileCreation()
        {
            var genders = profileService.GetGenders();
            var countries = profileService.GetCountries();

            ProfileViewModel viewModel = new ProfileViewModel();

            Dictionary<int, string> genderCollection = new Dictionary<int, string>();
            foreach (var gender in genders)
            {
                genderCollection.Add(gender.Id, gender.Name);
            }

            viewModel.Genders = genderCollection;

            Dictionary<int, string> countryCollection = new Dictionary<int, string>();
            foreach (var country in countries)
            {
                countryCollection.Add(country.Id, country.Name);
            }

            viewModel.Countries = countryCollection;

            return View(viewModel);
        }

        public async Task<ActionResult> Create(ProfileViewModel viewModel)
        {
            var currentUser = await GetCurrentUserAsync();
            DateTime birthday = new DateTime(viewModel.BirthYear.Value, viewModel.BirthMonth.Value, viewModel.BirthDayOfMonth.Value);
            profileService.CreateProfile(viewModel.SelectedGenderId.Value, viewModel.SelectedZipCodeId, viewModel.SelectedCityId.Value, currentUser.Id, birthday);
            return RedirectToIndex();
        }

        private ActionResult RedirectToIndex()
        {
            return RedirectToAction("index");
        }
    }
}