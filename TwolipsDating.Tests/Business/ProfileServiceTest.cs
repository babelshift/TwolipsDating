using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using TwolipsDating.Models;
using TwolipsDating.Business;
using System.Threading.Tasks;

namespace TwolipsDating.Tests.Business
{
    [TestClass]
    public class ProfileServiceTest
    {
        [TestMethod]
        public async Task Add_Tag_Suggestion()
        {
            var mockSet = new Mock<DbSet<TagSuggestion>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.TagSuggestions).Returns(mockSet.Object);

            ProfileService service = new ProfileService(mockContext.Object, new EmailService());
            int changes = await service.AddTagSuggestionAsync(1, 1, Guid.NewGuid().ToString());

            mockSet.Verify(m => m.Add(It.IsAny<TagSuggestion>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}
