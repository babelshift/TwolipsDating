using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class UserService : BaseService
    {
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
    }
}