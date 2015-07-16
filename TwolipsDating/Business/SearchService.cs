using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TwolipsDating.Models;
using System.Data.Entity;

namespace TwolipsDating.Business
{
    public class SearchService : BaseService
    {
        internal async Task<IReadOnlyCollection<Profile>> SearchProfilesByUserName(string userName)
        {
            var results = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.UserName.Contains(userName)
                                 select profiles).ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<Profile>> SearchProfilesByTagName(string userName)
        {
            var results = await (from profiles in db.Profiles
                                 join tagSuggestions in db.TagSuggestions on profiles.Id equals tagSuggestions.ProfileId
                                 where tagSuggestions.Tag.Name == userName
                                 select profiles).ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<Profile>> SearchProfilesByUserNameAndTagName(string userName, string tagName)
        {
            throw new NotImplementedException();
        }
    }
}