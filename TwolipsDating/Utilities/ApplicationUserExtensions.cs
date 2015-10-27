using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TwolipsDating.Models;
using TwolipsDating.Utilities;

namespace TwolipsDating.Utilities
{
    public static class ApplicationUserExtensions
    {
        public static void IncreasePoints(this ApplicationUser user, int points)
        {
            if (user != null)
            {
                user.CurrentPoints += points;
                user.LifetimePoints += points;
            }
        }

        public static string ToSEOName(this ApplicationUser user)
        {
            if (user == null)
            {
                return String.Empty;
            }

            return ToSEOName(user.UserName);
        }

        public static string ToSEOName(string userName)
        {
            return userName.ToSEOFriendlyString();
        }
    }
}