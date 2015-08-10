using System;
using System.Text.RegularExpressions;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    internal static class QuizExtensions
    {
        internal static string GetSEOName(this Quiz quiz)
        {
            if (String.IsNullOrEmpty(quiz.Name))
            {
                return String.Empty;
            }

            string root = String.Format("{0}", quiz.Name);
            return Regex.Replace(root.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }
    }
}