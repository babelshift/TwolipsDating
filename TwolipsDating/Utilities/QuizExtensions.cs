using System;
using System.Configuration;
using System.Text.RegularExpressions;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    internal static class QuizExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        internal static string GetSEOName(this Quiz quiz)
        {
            if (String.IsNullOrEmpty(quiz.Name))
            {
                return String.Empty;
            }

            string root = String.Format("{0}", quiz.Name);
            return Regex.Replace(root.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }

        internal static string GetImageUrl(this Quiz quiz)
        {
            if (quiz != null && quiz.ImageFileName != null)
            {
                return GetImagePath(quiz.ImageFileName);
            }

            return String.Empty;
        }

        internal static string GetImagePath(string fileName)
        {
            return String.Format("{0}/{1}", cdn, fileName);
        }
    }
}