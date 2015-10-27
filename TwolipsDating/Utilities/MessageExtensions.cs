using System;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class MessageExtensions
    {
        public static string GetSenderProfileThumbnailImagePath(this MessageConversation messageConversation)
        {
            string fileName = String.Empty;

            if (messageConversation != null)
            {
                fileName = messageConversation.SenderProfileImageFileName;
            }

            return ProfileExtensions.GetThumbnailImagePath(fileName);
        }

        public static string GetReceiverProfileThumbnailImagePath(this MessageConversation messageConversation)
        {
            string fileName = String.Empty;

            if (messageConversation != null)
            {
                fileName = messageConversation.ReceiverProfileImageFileName;
            }

            return ProfileExtensions.GetThumbnailImagePath(fileName);
        }
    }
}