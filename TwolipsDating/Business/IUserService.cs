using System;
using System.Threading.Tasks;
using TwolipsDating.Models;
namespace TwolipsDating.Business
{
    public interface IUserService : IBaseService
    {
        bool DoesUserHaveProfile(string userId);
        System.Threading.Tasks.Task<bool> DoesUserHaveProfileAsync(string userId);
        System.Threading.Tasks.Task<TwolipsDating.Models.EmailNotifications> GetEmailNotificationsForUserAsync(string userId);
        int? GetProfileId(string userId);
        System.Threading.Tasks.Task<int?> GetProfileIdAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.StoreTransactionLog>> GetStoreTransactionsAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyDictionary<int, TwolipsDating.Models.UserTitle>> GetTitlesOwnedByUserAsync(string currentUserId);
        System.Threading.Tasks.Task<bool> IsUserIgnoredByUserAsync(string sourceUserId, string targetUserId);
        System.Threading.Tasks.Task SaveEmailNotificationChangesAsync(string currentUserId, bool sendGiftNotifications, bool sendMessageNotifications, bool sendNewFollowerNotifications, bool sendTagNotifications, bool sendReviewNotifications);
        System.Threading.Tasks.Task SendGiftEmailNotificationAsync(string senderUserName, string senderProfileImagePath, string senderProfileUrl, string giftName, string giftImagePath, string receiverUserId, string receiverUserName, string receiverEmail);
        System.Threading.Tasks.Task SendMessageEmailNotificationAsync(string senderProfileImagePath, string senderUserName, string messageText, string conversationUrl, string receiverUserId, string receiverUserName, string receiverEmail);
        System.Threading.Tasks.Task SendNewFollowerEmailNotificationAsync(string followerProfileImagePath, string followerUserName, string followerProfileUrl, string followingUserId, string followingUserName, string followingEmail);
        System.Threading.Tasks.Task SendReviewEmailNotificationAsync(string senderProfileImagePath, string senderUserName, string reviewText, string senderProfileUrl, string receiverUserId, string receiverUserName, string receiverEmail);
        System.Threading.Tasks.Task SetUserLastLoginByEmailAsync(string email);
        System.Threading.Tasks.Task SetUserLastLoginByIdAsync(string userId);
        Task<ReferralCodeServiceResult> GenerateReferralCodeForUserAsync(string userId);

        Task RewardReferralAsync(ApplicationUser user, string referralCode);

        Task<bool> IsReferralCodeValidAsync(string referralCode);
    }
}
