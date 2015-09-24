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
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        internal static string GetIconPath(this Milestone milestone)
        {
            if (milestone != null && milestone.IconFileName != null)
            {
                return GetIconPath(milestone.IconFileName);
            }

            return String.Empty;
        }

        internal static string GetIconPath(string fileName)
        {
            return String.Format("{0}/{1}?5", cdn, fileName);
        }
    }
}