using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.Utilities;

namespace TwolipsDating.Business
{
    public class UserService : BaseService
    {
        public UserService() : base() { }
        
        public UserService(IIdentityMessageService emailService)
            : base(emailService) { }

        public UserService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService) { }

        /// <summary>
        /// Returns a boolean indicating if a user has ignored another user.
        /// </summary>
        /// <param name="sourceUserId"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        public async Task<bool> IsUserIgnoredByUserAsync(string sourceUserId, string targetUserId)
        {
            Debug.Assert(!String.IsNullOrEmpty(sourceUserId));
            Debug.Assert(!String.IsNullOrEmpty(targetUserId));

            var ignoredUserEntity = from ignoredUser in db.IgnoredUsers
                                    where ignoredUser.SourceUserId == sourceUserId
                                    where ignoredUser.TargetUserId == targetUserId
                                    select ignoredUser;

            var result = await ignoredUserEntity.FirstOrDefaultAsync();

            return result != null;
        }

        /// <summary>
        /// Returns a collection of all store transactions for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<StoreTransactionLog>> GetStoreTransactionsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var results = await (from transactions in db.StoreTransactions
                                 where transactions.UserId == userId
                                 select transactions).ToListAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a dictionary containing all titles that are owned by a user.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        internal async Task<IReadOnlyDictionary<int, UserTitle>> GetTitlesOwnedByUserAsync(string currentUserId)
        {
            Debug.Assert(!String.IsNullOrEmpty(currentUserId));

            var purchasedTitles = await (from titles in db.UserTitles
                                         where titles.UserId == currentUserId
                                         select titles).ToDictionaryAsync(t => t.StoreItemId, t => t);

            return new ReadOnlyDictionary<int, UserTitle>(purchasedTitles);
        }

        /// <summary>
        /// Returns the profile ID of the passed user ID. Returns null if the user has no profile.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal async Task<int?> GetProfileIdAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var profile = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.Id == userId
                                 select profiles).FirstOrDefaultAsync();

            if (profile != null)
            {
                return profile.Id;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a boolean indicating if a user has a profile.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal async Task<bool> DoesUserHaveProfileAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            return (await GetProfileIdAsync(userId)).HasValue;
        }

        internal async Task SendNewFollowerEmailNotificationAsync(string followerProfileImagePath, string followerUserName, string followerProfileUrl,
            string followingUserId, string followingUserName, string followingEmail)
        {
            var emailNotifications = await GetEmailNotificationsForUserAsync(followingUserId);

            // only send an email if the user wants to be notified of this event
            if (emailNotifications.SendNewFollowerNotifications)
            {
                IdentityMessage message = new IdentityMessage()
                {
                    Body = EmailTextHelper.NewFollowerEmail.GetBody(followingUserName, followerProfileImagePath, followerUserName, followerProfileUrl),
                    Destination = followingEmail,
                    Subject = EmailTextHelper.NewFollowerEmail.GetSubject(followerUserName)
                };
                await EmailService.SendAsync(message);
            }
        }

        internal async Task SendMessageEmailNotificationAsync(string senderProfileImagePath, string senderUserName, string messageText, string conversationUrl,
            string receiverUserId, string receiverUserName, string receiverEmail)
        {
            var emailNotifications = await GetEmailNotificationsForUserAsync(receiverUserId);

            // only send an email if the user wants to be notified of this event
            if (emailNotifications.SendMessageNotifications)
            {
                IdentityMessage message = new IdentityMessage()
                {
                    Body = EmailTextHelper.MessageEmail.GetBody(receiverUserName, senderProfileImagePath, senderUserName, messageText, conversationUrl),
                    Destination = receiverEmail,
                    Subject = EmailTextHelper.MessageEmail.GetSubject(senderUserName)
                };
                await EmailService.SendAsync(message);
            }
        }

        internal async Task SendGiftEmailNotificationAsync(string senderUserName, string senderProfileImagePath, string senderProfileUrl, string giftName, string giftImagePath, 
            string receiverUserId, string receiverUserName, string receiverEmail)
        {
            var emailNotifications = await GetEmailNotificationsForUserAsync(receiverUserId);

            // only send an email if the user wants to be notified of this event
            if (emailNotifications.SendGiftNotifications)
            {
                IdentityMessage message = new IdentityMessage()
                {
                    Body = EmailTextHelper.GiftEmail.GetBody(receiverUserName, giftImagePath, giftName, senderProfileImagePath, senderUserName, senderProfileUrl),
                    Destination = receiverEmail,
                    Subject = EmailTextHelper.GiftEmail.GetSubject(senderUserName)
                };
                await EmailService.SendAsync(message);
            }
        }

        internal async Task<EmailNotifications> GetEmailNotificationsForUserAsync(string userId)
        {
            var emailNotifications = from emailNotification in db.EmailNotifications
                                     where emailNotification.ApplicationUserId == userId
                                     select emailNotification;

            var result = await emailNotifications.FirstOrDefaultAsync();

            if(result == null)
            {
                result = new EmailNotifications()
                {
                    ApplicationUserId = userId,
                    SendGiftNotifications = false,
                    SendMessageNotifications = false,
                    SendNewFollowerNotifications = false,
                    SendTagNotifications = false
                };
            }

            return result;
        }

        internal async Task SaveEmailNotificationChangesAsync(string currentUserId, 
            bool sendGiftNotifications, 
            bool sendMessageNotifications, 
            bool sendNewFollowerNotifications, 
            bool sendTagNotifications)
        {
            Debug.Assert(!String.IsNullOrEmpty(currentUserId));

            EmailNotifications emailNotifications = await db.EmailNotifications.FindAsync(currentUserId);
            if(emailNotifications == null)
            {
                emailNotifications = new EmailNotifications();
                emailNotifications.ApplicationUserId = currentUserId;
                emailNotifications.SendGiftNotifications = sendGiftNotifications;
                emailNotifications.SendMessageNotifications = sendMessageNotifications;
                emailNotifications.SendNewFollowerNotifications = sendNewFollowerNotifications;
                emailNotifications.SendTagNotifications = sendTagNotifications;
                db.EmailNotifications.Add(emailNotifications);
            }
            else
            {
                emailNotifications.SendGiftNotifications = sendGiftNotifications;
                emailNotifications.SendMessageNotifications = sendMessageNotifications;
                emailNotifications.SendNewFollowerNotifications = sendNewFollowerNotifications;
                emailNotifications.SendTagNotifications = sendTagNotifications;
            }

            await db.SaveChangesAsync();
        }
    }
}