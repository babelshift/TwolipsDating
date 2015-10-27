using System;
using System.Configuration;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Utilities
{
    public static class GiftExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        public static string GetIconPath(this StoreItemViewModel storeItemViewModel)
        {
            if (storeItemViewModel != null && !String.IsNullOrEmpty(storeItemViewModel.ItemImagePath))
            {
                return GetImagePath(storeItemViewModel.ItemImagePath);
            }

            return String.Empty;
        }

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

        public static string GetIconPath(string fileName)
        {
            return GetImagePath(fileName);
        }
    }
}