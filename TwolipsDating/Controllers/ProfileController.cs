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
    public class ProfileController : Controller
    {
        private ApplicationUserManager _userManager;
        private ProfileService p = new ProfileService();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            var profile = p.GetProfile(user.Id);

            if (profile != null)
            {
                var profileViewModel = Mapper.Map<TwolipsDating.Models.Profile, ProfileViewModel>(profile);

                return View(profileViewModel);
            }
            else
            {
                var genders = p.GetGenders();
                var countries = p.GetCountries();

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
            p.CreateProfile(viewModel.SelectedGenderId.Value, viewModel.SelectedZipCodeId, viewModel.SelectedCityId.Value, user.Id, birthday);

            return RedirectToAction("index");
        }
    }
}