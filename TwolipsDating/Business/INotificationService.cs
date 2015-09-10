using System;
namespace TwolipsDating.Business
{
    public interface INotificationService : IBaseService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<TwolipsDating.Models.Announcement>> GetAnnouncementNotificationsAsync();
        System.Threading.Tasks.Task<int> GetMessageNotificationCountAsync(string userId);
    }
}
