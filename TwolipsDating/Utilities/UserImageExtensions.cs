using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class UserImageExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        public static string GetPath(this UserImage userImage)
        {
            if (userImage != null)
            {
                return String.Format("{0}/{1}", cdn, userImage.FileName);
            }

            return String.Empty;
        }

        public static string GetPath(string fileName)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                return String.Format("{0}/{1}", cdn, fileName);
            }

            return String.Empty;
        }

        public static string GetThumbnailPath(this UserImage userImage)
        {
            string realFileName = Path.GetFileNameWithoutExtension(userImage.FileName);
            string fileType = Path.GetExtension(userImage.FileName);

            return String.Format("{0}/{1}_{2}{3}", cdn, realFileName, "thumb", fileType);
        }
    }
}