using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TwolipsDating.Models;
using System.Data.Entity;

namespace TwolipsDating.Business
{
    public class NotificationService : BaseService
    {
        public async Task<IReadOnlyList<Announcement>> GetAnnouncementNotificationsAsync()
        {
            var announcementsList = from announcements in db.Announcements
                                    select announcements;

            return (await announcementsList.ToListAsync()).AsReadOnly();
        }

        public async Task<int> GetMessageNotificationCountAsync(string userId)
        {
            var messageCount = await (from message in db.Messages
                                      where message.ReceiverApplicationUserId == userId
                                      where message.MessageStatusId == (int)MessageStatusValue.Unread
                                      select message).CountAsync();

            return messageCount;
        }
    }
}