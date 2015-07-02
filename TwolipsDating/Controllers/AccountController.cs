﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.Business;
using TwolipsDating.ViewModels;
using AutoMapper;
using System.Collections.Generic;

namespace TwolipsDating.Controllers
{
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private UserService userService = new UserService();
        private StoreService storeService = new StoreService();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
			: base (userManager)
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

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);
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
                    ModelState.AddModelError("", "The entered username or password is incorrect.");
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
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
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

                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("confirmemail", "account", new { userId = user.Id, code = code }, Request.Url.Scheme);

                string mailSubject = "Confirm your Twolips Dating account";
                string mailBody = @"<img src=""http://www.twolipsdating.com/Content/twolipsicon-white.png"" width=""32"" height=""32"" style=""float: left;"" />
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 24px; margin-left: 35px;"">
    Twolips Dating - Find better dates and have a better time.
</div>
<br />
<div style=""border-top: 1px solid #4b4b4b; width: 650px;""></div>
<div style=""font-family: Helvetica,Arial,sans-serif; margin-left: 25px; width: 500px; margin-bottom: 15px;"">
    <p style=""font-weight: bold;"">Hello and thank you for registering for Twolips Dating!</p>

    <div style=""font-size: 12px; color: #4b4b4b;"">
        <p>We're sending you this message to confirm your new account. If you think that this message is not intended for you, please ignore it.</p>

        <p>
            If you're the right person, please <a href=" + callbackUrl + @">click here to confirm your account</a> as soon as possible to begin contributing to the community.
            Please be aware that you will be unable to login and perform certain activities until your account is confirmed.
        </p>
    </div>
</div>
<div style=""border-top: 1px solid #4b4b4b; width: 650px;""></div>
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
    <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""https://www.twolipsdating.com/"">Home</a>
    <p><small>Twolips Dating, Orlando, FL, USA</small></p>
</div>
";
                await UserManager.SendEmailAsync(user.Id, mailSubject, mailBody);
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
                    return RedirectToAction("index", "home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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

            if(result.Succeeded)
            {
                IdentityMessage message = new IdentityMessage();
                message.Destination = "admin@twolipsdating.com";
                message.Subject = "A new user has confirmed their e-mail address";
                message.Body = String.Format("UserId = {0}", userId);
                await UserManager.EmailService.SendAsync(message);
            }

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
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

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
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
                    //DateCreated = DateTime.Now,
                    //DateLastLogin = DateTime.Now,
                    //EmailConfirmed = true,
                    //IsActive = true,
                    //DisplayName = model.UserName
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
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
            await SetHeaderCountsAsync();

            string currentUserId = await GetCurrentUserIdAsync();

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
                    ItemName = title.Value.Title.Name,
                    ItemCost = title.Value.Title.PointPrice,
                    ItemCount = 1,
                    ItemType = "Title",
                    TotalCost = title.Value.Title.PointPrice
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
        #endregion
    }
}