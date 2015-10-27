using System;
using System.Configuration;
using System.IO;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    internal static class QuizExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];    // URL to the image CDN
        private const int imageVersion = 1;                                                 // version of the image to display from the CDN
        private const int thumbnailImageVersion = 1;                                        // version of the thumbnail to display from the CDN
        private const string thumbnailSuffix = "thumb";                                     // string to append to end of image to get thumbnail
        private const string placeholderFileName = "PlaceholderQuiz.jpg";                   // if the quiz has no image, this is displayed

        #region SEO Name

        /// <summary>
        /// Returns an SEO / URL friendly string based on the passed quiz name. For example, "The Squares" would return "the-squares".
        /// </summary>
        /// <param name="quizName"></param>
        /// <returns></returns>
        internal static string ToSEOName(string quizName)
        {
            return quizName.ToSEOFriendlyString();
        }

        internal static string ToSEOName(this Quiz quiz)
        {
            if (quiz == null)
            {
                return String.Empty;
            }

            return ToSEOName(quiz.Name);
        }

        #endregion SEO Name

        #region Image Path

        internal static string GetImagePath(this Quiz quiz)
        {
            if (quiz != null)
            {
                return GetImagePath(quiz.ImageFileName);
            }
            else
            {
                return GetImagePath(placeholderFileName);
            }
        }

        internal static string GetImagePath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = placeholderFileName;
            }

            return String.Format("{0}/{1}?{2}", cdn, fileName, imageVersion);
        }

        #endregion Image Path

        #region Thumbnail Image Path

        internal static string GetThumbnailImagePath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = placeholderFileName;
            }

            string realFileName = Path.GetFileNameWithoutExtension(fileName);
            string fileType = Path.GetExtension(fileName);

            return String.Format("{0}/{1}_{2}{3}?{4}", cdn, realFileName, thumbnailSuffix, fileType, thumbnailImageVersion);
        }

        internal static string GetThumbnailImagePath(this Quiz quiz)
        {
            if (quiz != null)
            {
                return GetThumbnailImagePath(quiz.ImageFileName);
            }
            else
            {
                return GetThumbnailImagePath(placeholderFileName);
            }
        }

        #endregion Thumbnail Image Path
    }
}