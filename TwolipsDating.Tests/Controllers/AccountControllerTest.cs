using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwolipsDating;
using TwolipsDating.Controllers;
using System.Threading.Tasks;
using Moq;
using TwolipsDating.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.Owin;
using TwolipsDating.Business;
using System.Web.Routing;
using System.Security.Principal;

namespace TwolipsDating.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void GET_Register_ReturnsView()
        {
            AccountController controller = new AccountController();

            var result = controller.Register() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, String.Empty);
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

            userManager.Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>(), password)).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(a => a.GenerateEmailConfirmationTokenAsync(It.IsAny<string>())).ReturnsAsync(String.Empty);
            userManager.Setup(a => a.SendEmailAsync(It.IsAny<String>(), String.Empty, String.Empty)).Returns(Task.FromResult(false));

            userService.Setup(a => a.SaveEmailNotificationChangesAsync(It.IsAny<String>(), true, true, true, true, true)).Returns(Task.FromResult(false));

            signInManager.Setup(a => a.SignInAsync(It.IsAny<ApplicationUser>(), true, true)).Returns(Task.FromResult(false));


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

            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ApplicationPath).Returns("/");
            request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/a", UriKind.Absolute));
            request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            var response = new Mock<HttpResponseBase>();
            response.Setup(x => x.ApplyAppPathModifier(Moq.It.IsAny<String>())).Returns((String url) => url);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);

            var identity = new Mock<IIdentity>();
            identity.Setup(x => x.Name).Returns("justin");
            identity.Setup(x => x.IsAuthenticated).Returns(true);
            context.SetupGet(x => x.User.Identity).Returns(identity.Object);

            var user = new Mock<ApplicationUser>();
            user.SetupGet(x => x.Points).Returns(0);
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<String>())).ReturnsAsync(user.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.Url = new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);

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

        //[TestMethod]
        //public void Home_Index_Returns_Empty_ViewResult_And_Model_If_Unauthorized()
        //{
        //    HomeController controller = new HomeController();
        //    var mock = new Mock<ControllerContext>();
        //    mock.SetupGet(x => x.HttpContext.User.Identity.Name).Returns(String.Empty);
        //    mock.SetupGet(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);
        //    controller.ControllerContext = mock.Object;

        //    var result = (ViewResult)((controller.Index((int?)null) as Task<ActionResult>).Result);
        //    Assert.IsNotNull(result);
        //    Assert.IsNotNull(result.ViewData.Model);
        //}

        //[TestMethod]
        //public void Home_Index_Shows_Dashboard_If_Authorized()
        //{
        //    HomeController controller = new HomeController();
        //    var mock = new Mock<ControllerContext>();
        //    mock.SetupGet(x => x.HttpContext.User.Identity.Name).Returns("justin");
        //    mock.SetupGet(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
        //    mock.SetupGet(x => x.HttpContext.Session["CurrentUserId"]).Returns(String.Empty);
        //    controller.ControllerContext = mock.Object;

        //    var result = (ViewResult)((controller.Index((int?)null) as Task<ActionResult>).Result);
        //    Assert.IsNotNull(result);
        //    Assert.IsNotNull(result.ViewData.Model);
        //    Assert.AreEqual(result.ViewName, "dashboard");
        //}
    }
}
