using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        public static NotificationService Create(IdentityFactoryOptions<NotificationService> options, IOwinContext context)
        {
            var service = new NotificationService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

        /// <summary>
        /// Returns a collection of announcements.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<Announcement>> GetAnnouncementNotificationsAsync()
        {
            var announcementsList = from announcements in db.Announcements
                                    select announcements;

            var results = await announcementsList.ToListAsync();
            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a count of message notifications for a user. Count includes only unread messages and from users that are marked as active.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> GetMessageNotificationCountAsync(string userId)
        {
            var messageCount = await (from message in db.Messages
                                      where message.ReceiverApplicationUserId == userId
                                      where message.MessageStatusId == (int)MessageStatusValue.Unread
                                      where message.SenderApplicationUser.IsActive
                                      select message).CountAsync();

            return messageCount;
        }
    }
}