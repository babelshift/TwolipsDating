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
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            profileService.SendMessage(user.Id, viewModel.ProfileUserId, viewModel.MessageSubject, viewModel.MessageBody);

            return RedirectToAction("index");
        }

        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            var profile = profileService.GetProfile(user.Id);

            if (profile != null)
            {
                var profileViewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);
                SetUnreadCountsInViewBag(profileService, user);

                return View(profileViewModel);
            }
            else
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
        }

        public async Task<ActionResult> Create(ProfileViewModel viewModel)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            DateTime birthday = new DateTime(viewModel.BirthYear.Value, viewModel.BirthMonth.Value, viewModel.BirthDayOfMonth.Value);
            profileService.CreateProfile(viewModel.SelectedGenderId.Value, viewModel.SelectedZipCodeId, viewModel.SelectedCityId.Value, user.Id, birthday);

            return RedirectToAction("index");
        }
    }
}