using System;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class QuizCategoryExtensions
    {
        internal static string ToSEOName(this QuizCategory quizCategory)
        {
            if (quizCategory == null)
            {
                return String.Empty;
            }

            return ToSEOName(quizCategory.Name);
        }

        internal static string ToSEOName(string categoryName)
        {
            return categoryName.ToSEOFriendlyString();
        }
    }
}