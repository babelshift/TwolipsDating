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

namespace TwolipsDating.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Home_Index_Returns_Empty_ViewResult_And_Model_If_Unauthorized()
        {
            HomeController controller = new HomeController();
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(x => x.HttpContext.User.Identity.Name).Returns(String.Empty);
            mock.SetupGet(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);
            controller.ControllerContext = mock.Object;

            var result = (ViewResult)((controller.Index((int?)null) as Task<ActionResult>).Result);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [TestMethod]
        public void Home_Index_Shows_Dashboard_If_Authorized()
        {
            HomeController controller = new HomeController();
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(x => x.HttpContext.User.Identity.Name).Returns("justin");
            mock.SetupGet(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            mock.SetupGet(x => x.HttpContext.Session["CurrentUserId"]).Returns(String.Empty);
            controller.ControllerContext = mock.Object;

            var result = (ViewResult)((controller.Index((int?)null) as Task<ActionResult>).Result);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(result.ViewName, "dashboard");
        }
    }
}
