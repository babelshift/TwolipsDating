﻿using System.Collections.Generic;
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
    }
}