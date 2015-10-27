using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class ProfileExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];    // URL to the image CDN
        private const int imageVersion = 2;                                                 // version of the image to display from the CDN
        private const int thumbnailImageVersion = 2;                                        // version of the thumbnail to display from the CDN
        private const string thumbnailSuffix = "thumb";                                     // string to append to end of image to get thumbnail
        private const string placeholderFileName = "Placeholder.png";                       // if the profile has no image, this is displayed

        #region SEO

        public static string ToSEOName(this Profile profile)
        {
            if (profile == null || profile.ApplicationUser == null)
            {
                return String.Empty;
            }

            return ToSEOName(profile.ApplicationUser.UserName);
        }

        public static string ToSEOName(string userName)
        {
            return userName.ToSEOFriendlyString();
        }

        #endregion SEO

        #region Image Path

        public static string GetImagePath(this Profile profile)
        {
            if (profile != null && profile.UserImage != null)
            {
                return GetImagePath(profile.UserImage.FileName);
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

        #endregion

        #region Thumbnail Image Path

        public static string GetThumbnailImagePath(this Profile profile)
        {
            if (profile != null && profile.UserImage != null)
            {
                return GetThumbnailImagePath(profile.UserImage.FileName);
            }
            else
            {
                return GetThumbnailImagePath(placeholderFileName);
            }
        }

        public static string GetThumbnailImagePath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = placeholderFileName;
            }

            string realFileName = Path.GetFileNameWithoutExtension(fileName);
            string fileType = Path.GetExtension(fileName);

            return String.Format("{0}/{1}_{2}{3}?{4}", cdn, realFileName, thumbnailSuffix, fileType, thumbnailImageVersion);
        }

        #endregion
    }
}