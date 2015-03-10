using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwolipsDating;
using TwolipsDating.Controllers;
using System.Threading.Tasks;

namespace TwolipsDating.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ActionResult result = (controller.Index() as Task<ActionResult>).Result;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
