using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwolipsDating.Models;
using TwolipsDating.Business;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TwolipsDating.Tests.Business
{
    [TestClass]
    public class SearchServiceTest
    {
        private ApplicationDbContext db;
        private SearchService service;

        [TestInitialize]
        public void TestInitialize()
        {
            db = ApplicationDbContext.Create();
            service = new SearchService(db, new EmailService());
        }

        //[TestMethod]
        //public async Task Search_User_Justin_Not_Null()
        //{
        //    var results = await service.GetProfilesByUserNameAsync("justin");
        //    Assert.IsNotNull(results);
        //}

        //[TestMethod]
        //public async Task Search_User_Justin_Exists()
        //{
        //    var results = await service.GetProfilesByUserNameAsync("justin");
        //    Assert.AreEqual(results.Count, 1);
        //}
    }
}
