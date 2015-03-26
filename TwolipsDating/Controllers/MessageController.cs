using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
using TwolipsDating.Utilities;

namespace TwolipsDating.Controllers
{
    public class MessageController : BaseController
    {
        public async Task<ActionResult> Received()
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var messages = await ProfileService.GetMessagesByUserAsync(currentUserId);

            List<ReceivedMessageViewModel> receivedMessages = new List<ReceivedMessageViewModel>();
            foreach (var message in messages)
            {
                if (message.ReceiverApplicationUserId == currentUserId)
                {
                    string senderProfileImagePath = message.SenderApplicationUser.Profile.UserImage != null 
                        ? String.Format("{0}/{1}", CDN, message.SenderApplicationUser.Profile.UserImage.FileName)
                        : String.Empty;
                    
                    receivedMessages.Add(new ReceivedMessageViewModel()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        DateSent = message.DateSent,
                        SenderName = message.SenderApplicationUser.UserName,
                        TimeAgo = message.DateSent.GetTimeAgo(),
                        SenderProfileImagePath = senderProfileImagePath,
                        SenderProfileId = message.SenderApplicationUser.Profile.Id
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                ReceivedMessages = receivedMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Received
            };

            await SetUnreadCountsInViewBag();

            return View("index", viewModel);
        }

        public async Task<ActionResult> Sent()
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var messages = await ProfileService.GetMessagesByUserAsync(currentUserId);

            List<SentMessageViewModel> sentMessages = new List<SentMessageViewModel>();
            foreach (var message in messages)
            {
                if (message.SenderApplicationUserId == currentUserId)
                {
                    string receiverProfileImagePath = message.ReceiverApplicationUser.Profile.UserImage != null
                        ? String.Format("{0}/{1}", CDN, message.ReceiverApplicationUser.Profile.UserImage.FileName)
                        : String.Empty;

                    sentMessages.Add(new SentMessageViewModel()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        DateSent = message.DateSent,
                        ReceiverName = message.ReceiverApplicationUser.UserName,
                        TimeAgo = message.DateSent.GetTimeAgo(),
                        ReceiverProfileImagePath = receiverProfileImagePath,
                        ReceiverProfileId = message.ReceiverApplicationUser.Profile.Id
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                SentMessages = sentMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Sent
            };

            await SetUnreadCountsInViewBag();

            return View("index", viewModel);
        }
    }
}