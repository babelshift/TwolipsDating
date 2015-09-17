using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}