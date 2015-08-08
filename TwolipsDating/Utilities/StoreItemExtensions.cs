using System;
using System.Configuration;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
using System.Linq;

namespace TwolipsDating.Utilities
{
    public static class StoreItemExtensions
    {
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