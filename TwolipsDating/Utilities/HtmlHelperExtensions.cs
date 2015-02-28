using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace TwolipsDating.Utilities
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Return the Current Version from the AssemblyInfo.cs file.
        /// </summary>
        public static string CurrentVersion(this HtmlHelper helper)
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return version.ToString();
            }
            catch
            {
                return "?.?.?.?";
            }
        }
    }
}