using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TwolipsDating.Models;

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

        public static string GetSEOName(this ApplicationUser user)
        {
            if (user != null)
            {
                return GetSEOName(user.UserName);
            }
            else
            {
                return String.Empty;
            }
        }

        public static string GetSEOName(string userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                return String.Empty;
            }

            string root = String.Format("{0}", userName);
            return Regex.Replace(root.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }
    }
}