using System;
using System.Configuration;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class GiftExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        public static string GetIconPath(this StoreItem storeItem)
        {
            if (storeItem != null && !String.IsNullOrEmpty(storeItem.IconFileName))
            {
                return GetImagePath(storeItem.IconFileName);
            }

            return String.Empty;
        }

        private static string GetImagePath(string fileName)
        {
            return String.Format("{0}/{1}", cdn, fileName);
        }
    }
}