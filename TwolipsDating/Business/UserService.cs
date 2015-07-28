using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class UserService : BaseService
    {
        public async Task<bool> IsUserIgnoredByUserAsync(string sourceUserId, string targetUserId)
        {
            var ignoredUserEntity = from ignoredUser in db.IgnoredUsers
                                    where ignoredUser.SourceUserId == sourceUserId
                                    where ignoredUser.TargetUserId == targetUserId
                                    select ignoredUser;

            var result = await ignoredUserEntity.FirstOrDefaultAsync();

            return result != null;
        }

        public async Task<IReadOnlyCollection<StoreTransactionLog>> GetStoreTransactionsAsync(string userId)
        {
            var results = await (from transactions in db.StoreTransactions
                                 where transactions.UserId == userId
                                 select transactions).ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyDictionary<int, UserTitle>> GetTitlesOwnedByUserAsync(string currentUserId)
        {
            var purchasedTitles = await (from titles in db.UserTitles
                                         where titles.UserId == currentUserId
                                         select titles).ToDictionaryAsync(t => t.TitleId, t => t);

            return new ReadOnlyDictionary<int, UserTitle>(purchasedTitles);
        }

        /// <summary>
        /// Returns the profile ID of the passed user ID. Returns null if the user has no profile.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        internal async Task<int?> GetProfileIdAsync(string currentUserId)
        {
            var profile = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.Id == currentUserId
                                 select profiles).FirstOrDefaultAsync();

            if(profile != null)
            {
                return profile.Id;
            }
            else
            {
                return null;
            }
        }
    }
}