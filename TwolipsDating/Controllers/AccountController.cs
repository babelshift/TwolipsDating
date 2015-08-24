using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private UserService userService = new UserService();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("index", "home");
            }

            string currentUserId = User.Identity.GetUserId();

            // only allow people to delete their own accounts
            if (currentUserId != id)
            {
                return RedirectToAction("index", "home");
            }

            var user = await UserManager.FindByIdAsync(id);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            user.EmailConfirmed = false;
            user.Logins.Clear();
            user.PhoneNumber = null;
            user.PhoneNumberConfirmed = false;
            user.Roles.Clear();
            user.UserName = GuidEncoder.Encode(user.Id);
            user.Email = String.Format("{0}@disabled.com", user.UserName);
            user.IsActive = false;
            user.Points = 0;
            IdentityResult updateUserResult = await UserManager.UpdateAsync(user);
            return RedirectToAction("index", "home");
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private async Task<SignInStatus> PasswordSignInByEmailAsync(string email, string password, bool rememberMe)
        {
            var user = await UserManager.FindByEmailAsync(email);

            if(user != null)
            {
                string userName = user.UserName;
                var result = await SignInManager.PasswordSignInAsync(user.UserName, password, rememberMe, shouldLockout: true);
                return result;
            }

            return SignInStatus.Failure;
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await PasswordSignInByEmailAsync(model.Email, model.Password, model.RememberMe);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "The account is locked. Please try again later or contact support for assistance.");
                    return View(model);

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "The entered email address or password is incorrect.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        internal async Task<IdentityResult> RegisterAccount(string userName, string password, string email)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                DateCreated = DateTime.Now,
                DateLastLogin = DateTime.Now,
                IsActive = true
            };
            var result = await UserManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                await SendRegistrationConfirmationEmail(user);
            }

            return result;
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await RegisterAccount(model.UserName, model.Password, model.Email);

                if (result.Succeeded)
                {
                    await SetNotificationsAsync();

                    return View("ConfirmEmailSent");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> SendConfirmEmail()
        {
            string userId = User.Identity.GetUserId();

            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            if(user.EmailConfirmed)
            {
                return RedirectToAction("index", "home");
            }
            else
            {
                await SendRegistrationConfirmationEmail(user);
            }

            await SetNotificationsAsync();

            return View("ConfirmEmailSent");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                // send email to admins letting them know someone confirmed their email
                await SendAdminEmailAfterConfirmationAsync(userId);

                // send email to user with welcome message
                await SendWelcomeEmailAsync(userId);
            }

            await SetNotificationsAsync();

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        private async Task SendWelcomeEmailAsync(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            IdentityMessage message = new IdentityMessage();
            message.Destination = user.Email;
            message.Subject = EmailTextHelper.WelcomeEmail.Subject;
            message.Body = EmailTextHelper.WelcomeEmail.GetBody();
            await UserManager.EmailService.SendAsync(message);
        }

        private async Task SendAdminEmailAfterConfirmationAsync(string userId)
        {
            IdentityMessage message = new IdentityMessage();
            message.Destination = "admin@twolipsdating.com";
            message.Subject = "A user has confirmed their e-mail address";
            message.Body = String.Format("UserId = {0}", userId);
            await UserManager.EmailService.SendAsync(message);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("resetpassword", "account", new { userId = user.Id, code = code }, Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset your Twolips Dating password", callbackUrl);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string userId, string code)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            ResetPasswordViewModel viewModel = new ResetPasswordViewModel()
            {
                UserName = user.UserName
            };

            return code == null ? View("Error") : View(viewModel);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        private async Task<ExternalLoginInfo> GetExternalLoginInfoWorkaroundAsync()
        {
            ExternalLoginInfo loginInfo = null;

            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

            if (result != null && result.Identity != null)
            {
                var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null)
                {
                    loginInfo = new ExternalLoginInfo()
                    {
                        DefaultUserName = result.Identity.Name == null ? "" : result.Identity.Name.Replace(" ", ""),
                        Login = new UserLoginInfo(idClaim.Issuer, idClaim.Value)
                    };
                }
            }
            return loginInfo;
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await GetExternalLoginInfoWorkaroundAsync();
            if (loginInfo == null)
            {
                Log.Error("External login information is null, returning to Login page.", String.Empty);

                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    DateCreated = DateTime.Now,
                    DateLastLogin = DateTime.Now,
                    //EmailConfirmed = true,
                    IsActive = true,
                    //DisplayName = model.UserName
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        await SendRegistrationConfirmationEmail(user);

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        private async Task SendRegistrationConfirmationEmail(ApplicationUser user)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("confirmemail", "account", new { userId = user.Id, code = code }, Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, EmailTextHelper.ConfirmationEmail.Subject, EmailTextHelper.ConfirmationEmail.GetBody(callbackUrl));
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["CurrentUserId"] = null;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public async Task<ActionResult> Points()
        {
            await SetNotificationsAsync();

            string currentUserId = User.Identity.GetUserId();

            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            // get transactions (expenses)
            var transactions = await userService.GetStoreTransactionsAsync(currentUserId);

            UserPointsViewModel viewModel = new UserPointsViewModel();
            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);
            viewModel.PointsCount = ViewBag.PointsCount;

            var storeTransactions = Mapper.Map<IReadOnlyCollection<StoreTransactionLog>, List<StoreTransactionViewModel>>(transactions);

            await AddTitlesToStoreTransactions(storeTransactions, currentUserId);

            CalculateTotalPointsSpent(viewModel, storeTransactions);

            viewModel.StoreTransactions = storeTransactions
                .OrderByDescending(t => t.TransactionDate)
                .ToList()
                .AsReadOnly();

            return View(viewModel);
        }

        private static void CalculateTotalPointsSpent(UserPointsViewModel viewModel, List<StoreTransactionViewModel> storeTransactions)
        {
            int totalSpent = 0;

            foreach (var transaction in storeTransactions)
            {
                totalSpent += transaction.ItemCost;
            }

            viewModel.TotalSpent = totalSpent;
        }

        private async Task AddTitlesToStoreTransactions(List<StoreTransactionViewModel> storeTransactions, string currentUserId)
        {
            var titles = await userService.GetTitlesOwnedByUserAsync(currentUserId);

            foreach (var title in titles)
            {
                storeTransactions.Add(new StoreTransactionViewModel()
                {
                    TransactionDate = title.Value.DateObtained,
                    ItemName = title.Value.StoreItem.Name,
                    ItemCost = title.Value.StoreItem.PointPrice,
                    ItemCount = 1,
                    ItemType = "Title",
                    TotalCost = title.Value.StoreItem.PointPrice
                });
            }
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

                if (userService != null)
                {
                    userService.Dispose();
                    userService = null;
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}