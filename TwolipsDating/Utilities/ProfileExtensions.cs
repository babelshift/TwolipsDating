using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class ProfileExtensions
    {
        private static readonly string cdn = ConfigurationManager.AppSettings["cdnUrl"];

        public static string GetProfileImagePath(this Profile profile)
        {
            if (profile != null && profile.UserImage != null)
            {
                return GetImagePath(profile.UserImage.FileName);
            }

            return String.Empty;
        }

        public static string GetSenderProfileImagePath(this MessageConversation messageConversation)
        {
            if (messageConversation != null && !String.IsNullOrEmpty(messageConversation.SenderProfileImageFileName))
            {
                return GetImagePath(messageConversation.SenderProfileImageFileName);
            }

            return String.Empty;
        }

        public static string GetReceiverProfileImagePath(this MessageConversation messageConversation)
        {
            if (messageConversation != null && !String.IsNullOrEmpty(messageConversation.ReceiverProfileImageFileName))
            {
                return GetImagePath(messageConversation.ReceiverProfileImageFileName);
            }

            return String.Empty;
        }

        private static string GetImagePath(string fileName)
        {
            return String.Format("{0}/{1}", cdn, fileName);
        }
    }
}