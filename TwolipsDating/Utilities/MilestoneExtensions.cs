using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class MilestoneExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];    // URL to the image CDN
        private const int imageVersion = 5;                                                 // version of the image to display from the CDN
        private const string placeholderFileName = "PlaceholderAchievement.png";                   // if the milestone has no image, this is displayed

        internal static string GetImagePath(this Milestone milestone)
        {
            if (milestone != null)
            {
                return GetImagePath(milestone.IconFileName);
            }
            else
            {
                return GetImagePath(placeholderFileName);
            }
        }

        internal static string GetImagePath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = placeholderFileName;
            }

            return String.Format("{0}/{1}?{2}", cdn, fileName, imageVersion);
        }
    }
}