using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TwolipsDating.Business;
using TwolipsDating.Controllers;
using TwolipsDating.Models;

namespace TwolipsDating.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        #region Setup

        private static void SetupMockAuthenticatedIdentity(Mock<HttpContextBase> mockContext)
        {
            var identity = new Mock<IIdentity>();
            identity.Setup(x => x.Name).Returns("justin");
            identity.Setup(x => x.IsAuthenticated).Returns(true);
            mockContext.SetupGet(x => x.User.Identity).Returns(identity.Object);
        }

        private static AccountController GetMockAccountController(Mock<ApplicationUserManager> userManager, Mock<ApplicationSignInManager> signInManager, Mock<IUserService> userService, Mock<IMilestoneService> milestoneService, Mock<INotificationService> notificationService, Mock<IProfileService> profileService, Mock<ISearchService> searchService, Mock<IStoreService> storeService, Mock<ITriviaService> triviaService, Mock<IViolationService> violationService, Mock<IDashboardService> dashboardService)
        {
            var controller = new AccountController(
                userManager.Object,
                signInManager.Object,
                profileService.Object,
                userService.Object,
                dashboardService.Object,
                milestoneService.Object,
                notificationService.Object,
                searchService.Object,
                storeService.Object,
                triviaService.Object,
                violationService.Object
            );
            return controller;
        }

        private static Mock<HttpContextBase> SetupMockHttpContext(Controller controller)
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ApplicationPath).Returns("/");
            request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/a", UriKind.Absolute));
            request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            var response = new Mock<HttpResponseBase>();
            response.Setup(x => x.ApplyAppPathModifier(Moq.It.IsAny<String>())).Returns((String url) => url);

            var mockContext = new Mock<HttpContextBase>();
            mockContext.SetupGet(x => x.Request).Returns(request.Object);
            mockContext.SetupGet(x => x.Response).Returns(response.Object);

            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            controller.ControllerContext = new ControllerContext(mockContext.Object, new RouteData(), controller);
            controller.Url = new UrlHelper(new RequestContext(mockContext.Object, new RouteData()), routes);

            SetupMockAuthenticatedIdentity(mockContext);

            return mockContext;
        }

        #endregion Setup

        #region Register

        [TestMethod]
        public void GET_Register_ReturnsView()
        {
            AccountController controller = new AccountController();

            var result = controller.Register(String.Empty) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, String.Empty);
        }

        [TestMethod]
        public async Task POST_ConfirmEmail_Success_RedirectTo_ConfirmEmail()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            var emailService = new Mock<IIdentityMessageService>();
            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            var userService = new Mock<IUserService>();
            var milestoneService = new Mock<IMilestoneService>();
            var notificationService = new Mock<INotificationService>();
            var profileService = new Mock<IProfileService>();
            var searchService = new Mock<ISearchService>();
            var storeService = new Mock<IStoreService>();
            var triviaService = new Mock<ITriviaService>();
            var violationService = new Mock<IViolationService>();
            var dashboardService = new Mock<IDashboardService>();

            var controller = GetMockAccountController(userManager, signInManager, userService, milestoneService, notificationService, profileService, searchService, storeService, triviaService, violationService, dashboardService);


            // mock up a fake email confirmation send
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<String>(), It.IsAny<String>())).ReturnsAsync(IdentityResult.Success);

            // mock up a fake email to admin about new user
            userManager.Setup(x => x.SendEmailAsync(It.IsAny<String>(), String.Empty, String.Empty)).Returns(Task.FromResult(false));

            // mock up a fake user returned
            var user = new Mock<ApplicationUser>();
            user.SetupGet(x => x.CurrentPoints).Returns(0);
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<String>())).ReturnsAsync(user.Object);

            var mockContext = SetupMockHttpContext(controller);

            var result = (await controller.ConfirmEmail(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "ConfirmEmail");
        }

        [TestMethod]
        public async Task POST_Register_Success_RedirectTo_ConfirmEmailSent()
        {
            string userName = "justin";
            string password = "password";
            string email = "email@email.com";

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            var userService = new Mock<IUserService>();
            var milestoneService = new Mock<IMilestoneService>();
            var notificationService = new Mock<INotificationService>();
            var profileService = new Mock<IProfileService>();
            var searchService = new Mock<ISearchService>();
            var storeService = new Mock<IStoreService>();
            var triviaService = new Mock<ITriviaService>();
            var violationService = new Mock<IViolationService>();
            var dashboardService = new Mock<IDashboardService>();

            // mock up a successful user creation
            userManager.Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>(), password)).ReturnsAsync(IdentityResult.Success);

            // mock up an empty email confirmation token
            userManager.Setup(a => a.GenerateEmailConfirmationTokenAsync(It.IsAny<string>())).ReturnsAsync(String.Empty);

            // mock up a fake email confirmation send
            userManager.Setup(a => a.SendEmailAsync(It.IsAny<String>(), String.Empty, String.Empty)).Returns(Task.FromResult(false));

            // mock up a fake email notification setup
            userService.Setup(a => a.SaveEmailNotificationChangesAsync(It.IsAny<String>(), true, true, true, true, true)).Returns(Task.FromResult(false));

            // mock up a fake sign in
            signInManager.Setup(a => a.SignInAsync(It.IsAny<ApplicationUser>(), true, true)).Returns(Task.FromResult(false));

            var controller = GetMockAccountController(userManager, signInManager, userService, milestoneService, notificationService, profileService, searchService, storeService, triviaService, violationService, dashboardService);

            var mockContext = SetupMockHttpContext(controller);

            var user = new Mock<ApplicationUser>();
            user.SetupGet(x => x.CurrentPoints).Returns(0);
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<String>())).ReturnsAsync(user.Object);

            notificationService.Setup(x => x.GetAnnouncementNotificationsAsync()).ReturnsAsync(new List<Announcement>().AsReadOnly());
            profileService.Setup(x => x.GetUnreviewedGiftTransactionsAsync(It.IsAny<String>())).ReturnsAsync(new List<GiftTransactionLog>().AsReadOnly());

            var viewModel = new RegisterViewModel()
            {
                UserName = userName,
                Password = password,
                Email = email
            };

            var result = (await controller.Register(viewModel)) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "ConfirmEmailSent");
        }

        #endregion Register

        #region Login

        [TestMethod]
        public void GET_Login_ReturnsView()
        {
            AccountController controller = new AccountController();

            var result = controller.Login(String.Empty) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, String.Empty);
        }

        [TestMethod]
        public async Task POST_Login_Success_RedirectsTo_Home_Index()
        {
            string password = "password";
            string email = "email@email.com";

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            var userService = new Mock<IUserService>();
            var milestoneService = new Mock<IMilestoneService>();
            var notificationService = new Mock<INotificationService>();
            var profileService = new Mock<IProfileService>();
            var searchService = new Mock<ISearchService>();
            var storeService = new Mock<IStoreService>();
            var triviaService = new Mock<ITriviaService>();
            var violationService = new Mock<IViolationService>();
            var dashboardService = new Mock<IDashboardService>();

            var controller = GetMockAccountController(userManager, signInManager, userService, milestoneService, notificationService, profileService, searchService, storeService, triviaService, violationService, dashboardService);

            var user = new Mock<ApplicationUser>();
            user.SetupGet(x => x.CurrentPoints).Returns(0);
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<String>())).ReturnsAsync(user.Object);
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<String>())).ReturnsAsync(user.Object);

            signInManager.Setup(x => x.PasswordSignInAsync(
                It.IsAny<String>(),
                It.IsAny<String>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInStatus.Success);

            userService.Setup(x => x.SetUserLastLoginByEmailAsync(It.IsAny<String>())).Returns(Task.FromResult(false));

            var mockContext = SetupMockHttpContext(controller);

            LoginViewModel viewModel = new LoginViewModel()
            {
                Email = email,
                Password = password
            };

            var result = (await controller.Login(viewModel, String.Empty)) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.RouteValues);
            Assert.IsNotNull(result.RouteValues["action"]);
            Assert.IsNotNull(result.RouteValues["controller"]);
            Assert.AreEqual(result.RouteValues["action"].ToString().ToLower(), "index");
            Assert.AreEqual(result.RouteValues["controller"].ToString().ToLower(), "home");
        }

        #endregion Login
    }
}