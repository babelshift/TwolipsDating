using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class ProfileExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        public static string GetProfileImagePath(this Profile profile)
        {
            if (profile != null && profile.UserImage != null)
            {
                return String.Format("{0}/{1}", cdn, profile.UserImage.FileName);
            }

            return String.Empty;
        }
    }
}