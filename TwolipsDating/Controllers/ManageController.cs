using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class ManageController : BaseController
    {
        private ApplicationSignInManager _signInManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationSignInManager signInManager)
        {
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private async Task SendEmailChangeConfirmationEmailAsync(ApplicationUser user)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("confirmemail", "account", new { userId = user.Id, code = code }, Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, EmailTextHelper.EmailChangedEmail.Subject, EmailTextHelper.EmailChangedEmail.GetBody(callbackUrl));
        }

        private async Task SendPasswordChangedConfirmationEmailAsync(ApplicationUser user)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("confirmemail", "account", new { userId = user.Id, code = code }, Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, EmailTextHelper.PasswordChangeEmail.Subject, EmailTextHelper.PasswordChangeEmail.GetBody(callbackUrl));
        }

        //
        // GET: /Manage/Index
        [ImportModelStateFromTempData]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            var currentUserId = User.Identity.GetUserId();

            string email = await UserManager.GetEmailAsync(currentUserId);

            IndexViewModel viewModel = new IndexViewModel()
            {
                Email = email,
                UserName = User.Identity.Name,
                DoesUserHaveProfile = false
            };

            var profile = await ProfileService.GetProfileAsync(currentUserId);

            if (profile != null)
            {
                var genders = await GetGendersAsync();
                viewModel.Genders = genders;
                viewModel.SelectedGenderId = profile.GenderId;

                viewModel.Months = CalendarHelper.GetMonths().ToDictionary(m => m.MonthNumber, m => m.MonthName);
                viewModel.Years = CalendarHelper.GetYears().ToDictionary(m => m, m => m);

                DateTime birthDate = profile.Birthday;
                viewModel.BirthDayOfMonth = birthDate.Day;
                viewModel.BirthMonth = birthDate.Month;
                viewModel.BirthYear = birthDate.Year;

                viewModel.Days = CalendarHelper.GetDaysOfMonth(birthDate.Month).ToDictionary(m => m, m => m);

                viewModel.CurrentLocation = profile.Location;

                viewModel.DoesUserHaveProfile = true;
            }

            await SetNotificationsAsync();

            if (TempData["StatusMessage"] != null)
            {
                ViewBag.StatusMessage = TempData["StatusMessage"].ToString();
            }

            return View(viewModel);
        }

        private async Task<Dictionary<int, string>> GetGendersAsync()
        {
            Dictionary<int, string> genderCollection = new Dictionary<int, string>();

            var genders = await ProfileService.GetGendersAsync();
            foreach (var gender in genders)
            {
                genderCollection.Add(gender.Id, gender.Name);
            }

            return genderCollection;
        }

        [HttpPost, ValidateAntiForgeryToken, ExportModelStateToTempData]
        public async Task<ActionResult> Index(IndexViewModel model)
        {
            await SetNotificationsAsync();

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                bool isNewUserName = SetupUpdateUserName(model, user);
                bool isNewEmail = SetupUpdateUserEmail(model.Email, user);
                bool isNewGender = false;
                bool isNewLocation = false;
                bool isNewBirthdate = false;

                // user has a profile, update it and setup the viewmodel to display on the view
                if (user.Profile != null)
                {
                    isNewBirthdate = SetupUpdateBirthdate(model, user);
                    isNewGender = SetupUpdateGender(model, user);

                    if (!String.IsNullOrEmpty(model.SelectedLocation))
                    {
                        isNewLocation = await SetupUpdateLocationAsync(model, user);
                    }
                }

                // if the user has chagned user name or email address, update the user entity
                if (isNewUserName || isNewEmail || isNewGender || isNewLocation || isNewBirthdate)
                {
                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);

                        TempData["StatusMessage"] = "Your account has been updated.";

                        // user changed email address, send confirmation
                        if (isNewEmail)
                        {
                            await SendEmailChangeConfirmationEmailAsync(user);
                            TempData["StatusMessage"] += " Remember to confirm your new email address.";
                        }

                        model.CurrentLocation = user.Profile.GeoCity.ToFullLocationString();
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            return RedirectToIndex();
        }

        private bool SetupUpdateBirthdate(IndexViewModel model, ApplicationUser user)
        {
            // if the user is changing his email, we need to re-confirm
            bool isNewBirthdate = false;
            DateTime birthDate = new DateTime(model.BirthYear.Value, model.BirthMonth.Value, model.BirthDayOfMonth.Value);
            if (user.Profile.Birthday != birthDate)
            {
                isNewBirthdate = true;
                user.Profile.Birthday = birthDate;
            }
            return isNewBirthdate;
        }

        private async Task<bool> SetupUpdateLocationAsync(IndexViewModel model, ApplicationUser user)
        {
            string[] location = model.SelectedLocation.Split(',');
            string cityName = location[0].Trim();
            string stateAbbreviation = location[1].Trim();
            string countryName = location[2].Trim();

            int cityId = await ProfileService.GetGeoCityIdAsync(cityName, stateAbbreviation, countryName);

            bool isNewLocation = false;

            if (model.CurrentLocation != model.SelectedLocation)
            {
                isNewLocation = true;
                user.Profile.GeoCityId = cityId;
            }

            return isNewLocation;
        }

        private bool SetupUpdateGender(IndexViewModel model, ApplicationUser user)
        {
            // if the user is changing his email, we need to re-confirm
            bool isNewGender = false;
            if (user.Profile.GenderId != model.SelectedGenderId)
            {
                isNewGender = true;
                user.Profile.GenderId = model.SelectedGenderId.Value;
            }
            return isNewGender;
        }

        private static bool SetupUpdateUserEmail(string newEmail, ApplicationUser user)
        {
            // if the user is changing his email, we need to re-confirm
            bool isNewEmail = false;
            if (user.Email != newEmail)
            {
                isNewEmail = true;
                user.EmailConfirmed = false;
                user.Email = newEmail;
            }
            return isNewEmail;
        }

        private static bool SetupUpdateUserName(IndexViewModel model, ApplicationUser user)
        {
            bool isNewUserName = false;
            if (user.UserName != model.UserName)
            {
                isNewUserName = true;
                user.UserName = model.UserName;
            }
            return isNewUserName;
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Externals", new { Message = message });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/ChangePassword
        [ImportModelStateFromTempData]
        public async Task<ActionResult> Settings(ManageMessageId? message)
        {
            await SetNotificationsAsync();

            var currentUser = User.Identity.GetUserId();
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var model = new SettingsViewModel
            {
                IndexViewModel = new IndexViewModel()
                {
                    HasPassword = HasPassword(),
                    Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId())
                }
            };

            if (TempData["StatusMessage"] != null)
            {
                ViewBag.StatusMessage = TempData["StatusMessage"].ToString();
            }

            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost, ValidateAntiForgeryToken, ExportModelStateToTempData]
        public async Task<ActionResult> Settings(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("settings");
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.ChangePasswordViewModel.OldPassword, model.ChangePasswordViewModel.NewPassword);

            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // user needs to confirm email address after changing password
                    user.EmailConfirmed = false;
                    await UserManager.UpdateAsync(user);
                    await SendEmailChangeConfirmationEmailAsync(user);
                }
                TempData["StatusMessage"] = "Your password has been changed. As a safety measure, you will now be required to reconfirm your email address.";
            }
            else
            {
                AddErrors(result);
            }

            return RedirectToAction("settings");
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            // we don't want to allow the user to remove an external login if that's the only login they have
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // GET: /Manage/ManageLogins
        [ImportModelStateFromTempData]
        public async Task<ActionResult> Externals(ManageMessageId? message)
        {
            await SetNotificationsAsync();

            var currentUser = User.Identity.GetUserId();
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }

            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;

            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        [ExportModelStateToTempData]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                Log.Error("External login information is null when linking an account.", String.Empty);

                return RedirectToAction("Externals", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);

            Log.Info(String.Format("UserId: {0}, Provider: {1}, Key: {2}", User.Identity.GetUserId(), loginInfo.Login.LoginProvider, loginInfo.Login.ProviderKey));

            foreach (string error in result.Errors)
            {
                Log.Error(error, String.Empty);
            }

            return result.Succeeded ? RedirectToAction("Externals") : RedirectToAction("Externals", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion Helpers

        [ImportModelStateFromTempData]
        public async Task<ActionResult> Notifications()
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            var emailNotifications = await UserService.GetEmailNotificationsForUserAsync(currentUserId);

            ManageNotificationsViewModel viewModel = Mapper.Map<EmailNotifications, ManageNotificationsViewModel>(emailNotifications);

            if (TempData["StatusMessage"] != null)
            {
                ViewBag.StatusMessage = TempData["StatusMessage"].ToString();
            }

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, ExportModelStateToTempData]
        public async Task<ActionResult> Notifications(ManageNotificationsViewModel viewModel)
        {
            await SetNotificationsAsync();

            if (ModelState.IsValid)
            {

                var currentUserId = User.Identity.GetUserId();

                await UserService.SaveEmailNotificationChangesAsync(currentUserId,
                    viewModel.SendGiftNotifications,
                    viewModel.SendMessageNotifications,
                    viewModel.SendNewFollowerNotifications,
                    viewModel.SendTagNotifications,
                    viewModel.SendReviewNotifications);

                TempData["StatusMessage"] = "Your notifications have been changed.";
            }

            return RedirectToAction("notifications");
        }
    }
}