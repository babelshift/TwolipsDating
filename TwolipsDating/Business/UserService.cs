using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.Utilities;

namespace TwolipsDating.Business
{
    public class UserService : BaseService, IUserService
    {
        public UserService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        public static UserService Create(IdentityFactoryOptions<UserService> options, IOwinContext context)
        {
            var service = new UserService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

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
        public async Task<IReadOnlyDictionary<int, UserTitle>> GetTitlesOwnedByUserAsync(string currentUserId)
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
        public int? GetProfileId(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var profile = (from profiles in db.Profiles
                           where profiles.ApplicationUser.Id == userId
                           select profiles).FirstOrDefault();

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
        /// Returns the profile ID of the passed user ID. Returns null if the user has no profile.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int?> GetProfileIdAsync(string userId)
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
        public async Task<bool> DoesUserHaveProfileAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            return (await GetProfileIdAsync(userId)).HasValue;
        }

        public bool DoesUserHaveProfile(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            return GetProfileId(userId).HasValue;
        }

        public async Task SendNewFollowerEmailNotificationAsync(string followerProfileImagePath, string followerUserName, string followerProfileUrl,
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

        public async Task SendMessageEmailNotificationAsync(string senderProfileImagePath, string senderUserName, string messageText, string conversationUrl,
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

        public async Task SendGiftEmailNotificationAsync(string senderUserName, string senderProfileImagePath, string senderProfileUrl, string giftName, string giftImagePath,
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

        public async Task SendReviewEmailNotificationAsync(string senderProfileImagePath, string senderUserName, string reviewText, string senderProfileUrl,
            string receiverUserId, string receiverUserName, string receiverEmail)
        {
            var emailNotifications = await GetEmailNotificationsForUserAsync(receiverUserId);

            // only send an email if the user wants to be notified of this event
            if (emailNotifications.SendReviewNotifications)
            {
                IdentityMessage message = new IdentityMessage()
                {
                    Body = EmailTextHelper.ReviewEmail.GetBody(receiverUserName, senderProfileImagePath, senderUserName, reviewText, senderProfileUrl),
                    Destination = receiverEmail,
                    Subject = EmailTextHelper.ReviewEmail.GetSubject(senderUserName)
                };
                await EmailService.SendAsync(message);
            }
        }

        public async Task<EmailNotifications> GetEmailNotificationsForUserAsync(string userId)
        {
            var emailNotifications = from emailNotification in db.EmailNotifications
                                     where emailNotification.ApplicationUserId == userId
                                     select emailNotification;

            var result = await emailNotifications.FirstOrDefaultAsync();

            if (result == null)
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

        public async Task SaveEmailNotificationChangesAsync(string currentUserId,
            bool sendGiftNotifications,
            bool sendMessageNotifications,
            bool sendNewFollowerNotifications,
            bool sendTagNotifications,
            bool sendReviewNotifications)
        {
            Debug.Assert(!String.IsNullOrEmpty(currentUserId));

            EmailNotifications emailNotifications = await db.EmailNotifications.FindAsync(currentUserId);
            if (emailNotifications == null)
            {
                emailNotifications = new EmailNotifications();
                emailNotifications.ApplicationUserId = currentUserId;
                emailNotifications.SendGiftNotifications = sendGiftNotifications;
                emailNotifications.SendMessageNotifications = sendMessageNotifications;
                emailNotifications.SendNewFollowerNotifications = sendNewFollowerNotifications;
                emailNotifications.SendTagNotifications = sendTagNotifications;
                emailNotifications.SendReviewNotifications = sendReviewNotifications;
                db.EmailNotifications.Add(emailNotifications);
            }
            else
            {
                emailNotifications.SendGiftNotifications = sendGiftNotifications;
                emailNotifications.SendMessageNotifications = sendMessageNotifications;
                emailNotifications.SendNewFollowerNotifications = sendNewFollowerNotifications;
                emailNotifications.SendTagNotifications = sendTagNotifications;
                emailNotifications.SendReviewNotifications = sendReviewNotifications;
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Looks up a user based on the provided email address and sets the dat of their last login to now.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task SetUserLastLoginByEmailAsync(string email)
        {
            Debug.Assert(!String.IsNullOrEmpty(email));

            var user = await db.Users.FirstAsync(u => u.Email == email);
            user.DateLastLogin = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public async Task SetUserLastLoginByIdAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            string sql = @"update dbo.aspnetusers
                set datelastlogin = @dateNow
                where id = @userId";

            int count = await ExecuteAsync(sql, new { dateNow = DateTime.Now, userId });
        }

        public async Task<ReferralCodeServiceResult> GenerateReferralCodeForUserAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var code = GuidEncoder.Encode(Guid.NewGuid().ToString());

            bool success = false;

            try
            {
                db.Referrals.Add(new Referral()
                    {
                        Code = code,
                        UserId = userId,
                        IsRedeemed = false,
                        DateCreated = DateTime.Now
                    });

                success = (await db.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex)
            {
                Log.Error("UserService.GenerateReferralCodeForUserAsync", ex, new { userId, code });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.ReferralCodeNotGenerated);
            }

            return success ? ReferralCodeServiceResult.Success(code) : ReferralCodeServiceResult.Failed(ErrorMessages.ReferralCodeNotGenerated);
        }

        public async Task RewardReferralAsync(ApplicationUser userWhoReceivedReferral, string referralCode)
        {
            Debug.Assert(userWhoReceivedReferral != null);
            Debug.Assert(!String.IsNullOrEmpty(referralCode));

            var referral = await db.Referrals.FindAsync(referralCode);
            var userWhoSentReferral = referral.ApplicationUser;

            // reward the users somehow
            // points
            // title
            // achievement

            userWhoReceivedReferral.IncreasePoints(100);

            // only reward the referral user if they are still active
            if (userWhoSentReferral.IsActive)
            {
                userWhoSentReferral.IncreasePoints(100);
            }

            referral.IsRedeemed = true;

            await db.SaveChangesAsync();

            await MilestoneService.AwardAchievedMilestonesAsync(userWhoSentReferral.Id, (int)MilestoneTypeValues.ReferralSignUps);
        }

        public async Task<int> GetReferralsRedeemedCountAsync(string userId)
        {
            int count = await db.Referrals.CountAsync(x => x.IsRedeemed && x.UserId == userId);
            return count;
        }

        public async Task<bool> IsReferralCodeValidAsync(string referralCode)
        {
            Debug.Assert(!String.IsNullOrEmpty(referralCode));

            var referral = await db.Referrals.FindAsync(referralCode);

            bool success = false;

            if (referral == null)
            {
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.ReferralCodeDoesNotExist);
            }
            else if (referral.IsRedeemed)
            {
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.ReferralCodeAlreadyRedeemed);
            }
            else
            {
                success = true;
            }

            return success;
        }
    }
}