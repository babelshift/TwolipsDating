using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    internal static class QuizExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];
        private const string placeholderFileName = "PlaceholderQuiz.jpg";
        
        internal static string GetSEOName(this QuizCategory quizCategory)
        {
            if (String.IsNullOrEmpty(quizCategory.Name))
            {
                return String.Empty;
            }

            string root = String.Format("{0}", quizCategory.Name);
            return Regex.Replace(root.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }

        internal static string GetSEOName(this Quiz quiz)
        {
            if (String.IsNullOrEmpty(quiz.Name))
            {
                return String.Empty;
            }

            string root = String.Format("{0}", quiz.Name);
            return Regex.Replace(root.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }

        internal static string GetImagePath(this Quiz quiz)
        {
            if (quiz != null && quiz.ImageFileName != null)
            {
                return GetImagePath(quiz.ImageFileName);
            }

            return String.Empty;
        }

        internal static string GetImagePath(string fileName)
        {
            return String.Format("{0}/{1}?1", cdn, fileName);
        }

        public static string GetThumbnailImagePath(string fileName)
        {
            return GetActualThumbnailImagePath(fileName);
        }

        private static string GetActualThumbnailImagePath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = placeholderFileName;
            }

            string realFileName = Path.GetFileNameWithoutExtension(fileName);
            string fileType = Path.GetExtension(fileName);

            return String.Format("{0}/{1}_{2}{3}?1", cdn, realFileName, "thumb", fileType);
        }

        public static string GetThumbnailImagePath(this Quiz quiz)
        {
            if (quiz != null && quiz.ImageFileName != null)
            {
                return GetActualThumbnailImagePath(quiz.ImageFileName);
            }
            else
            {
                return GetActualThumbnailImagePath(placeholderFileName);
            }
        }
    }
}