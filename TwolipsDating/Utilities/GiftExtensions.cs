using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class GiftExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        public static string GetIconPath(this Gift gift)
        {
            if (gift != null && !String.IsNullOrEmpty(gift.IconFileName))
            {
                return GetImagePath(gift.IconFileName);
            }

            return String.Empty;
        }

        private static string GetImagePath(string fileName)
        {
            return String.Format("{0}/{1}", cdn, fileName);
        }
    }
}