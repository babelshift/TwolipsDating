using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class ProfileExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];
        private const string placeholderFileName = "Placeholder.png";

        public static string GetProfileImagePath(string fileName)
        {
            return GetImagePath(fileName);
        }

        public static string GetProfileImagePath(this Profile profile)
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

        public static string GetSenderProfileImagePath(this MessageConversation messageConversation)
        {
            if (messageConversation != null && !String.IsNullOrEmpty(messageConversation.SenderProfileImageFileName))
            {
                return GetImagePath(messageConversation.SenderProfileImageFileName);
            }
            else
            {
                return GetImagePath(placeholderFileName);
            }
        }

        public static string GetReceiverProfileImagePath(this MessageConversation messageConversation)
        {
            if (messageConversation != null && !String.IsNullOrEmpty(messageConversation.ReceiverProfileImageFileName))
            {
                return GetImagePath(messageConversation.ReceiverProfileImageFileName);
            }
            else
            {
                return GetImagePath(placeholderFileName);
            }
        }

        private static string GetImagePath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = placeholderFileName;
            }

            string realFileName = Path.GetFileNameWithoutExtension(fileName);
            string fileType = Path.GetExtension(fileName);
            
            return String.Format("{0}/{1}_{2}{3}", cdn, realFileName, "thumb", fileType);
        }

        public static string GetSEOProfileName(string userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                return String.Empty;
            }

            string root = String.Format("{0}", userName);
            return Regex.Replace(root.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }

        public static string ToSEOString(this string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return s;
            }

            return Regex.Replace(s.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }
    }
}