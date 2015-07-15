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
        public async Task<IReadOnlyCollection<Profile>> SearchProfiles(string userName)
        {
            var results = await (from profiles in db.Profiles
                          where profiles.ApplicationUser.UserName.Contains(userName) 
                          select profiles).ToListAsync();

            return results.AsReadOnly();
        }
    }
}