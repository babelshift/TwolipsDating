using System;
using System.Configuration;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
using System.Linq;

namespace TwolipsDating.Utilities
{
    public static class StoreItemExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];
        private const string placeholderFileName = "PlaceholderGift.png";                   // if the gift has no image, this is displayed

        public static string GetImagePath(this StoreItem storeItem)
        {
            if (storeItem != null)
            {
                return GetImagePath(storeItem.IconFileName);
            }
            else
            {
                return GetImagePath(placeholderFileName);
            }
        }

        public static string GetImagePath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = placeholderFileName;
            }

            return String.Format("{0}/{1}", cdn, fileName);
        }

        public static double? GetDiscountIfAvailable(this StoreItem storeItem)
        {
            var sale = storeItem.StoreSales.FirstOrDefault(f => DateTime.Now >= f.DateStart && DateTime.Now <= f.DateEnd);
            return sale != null ? (double?)sale.Discount : null;
        }

        public static DateTime? GetDateSaleEndsIfAvailable(this StoreItem storeItem)
        {
            var sale = storeItem.StoreSales.FirstOrDefault(f => DateTime.Now >= f.DateStart && DateTime.Now <= f.DateEnd);
            return sale != null ? (DateTime?)sale.DateEnd : null;
        }
    }
}