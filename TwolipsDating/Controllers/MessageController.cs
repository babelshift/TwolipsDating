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
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            await SetUnreadCountsInViewBag();

            var messages = await ProfileService.GetMessagesByUserAsync(user.Id);

            List<ReceivedMessageViewModel> receivedMessages = new List<ReceivedMessageViewModel>();
            List<SentMessageViewModel> sentMessages = new List<SentMessageViewModel>();
            foreach (var message in messages)
            {
                if (message.ReceiverApplicationUserId == user.Id)
                {
                    receivedMessages.Add(new ReceivedMessageViewModel()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        DateSent = message.DateSent,
                        SenderName = message.SenderApplicationUser.UserName,
                        TimeAgo = message.DateSent.GetTimeAgo(),
                        SenderProfileImagePath = String.Format("{0}/{1}", CDN, message.SenderApplicationUser.Profile.UserImage.FileName),
                        SenderProfileId = message.SenderApplicationUser.Profile.Id
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                ReceivedMessages = receivedMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Received
            };

            return View("index", viewModel);
        }

        public async Task<ActionResult> Sent()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            await SetUnreadCountsInViewBag();

            var messages = await ProfileService.GetMessagesByUserAsync(user.Id);

            List<SentMessageViewModel> sentMessages = new List<SentMessageViewModel>();
            foreach (var message in messages)
            {
                if (message.SenderApplicationUserId == user.Id)
                {
                    sentMessages.Add(new SentMessageViewModel()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        DateSent = message.DateSent,
                        ReceiverName = message.ReceiverApplicationUser.UserName,
                        TimeAgo = message.DateSent.GetTimeAgo(),
                        ReceiverProfileImagePath = String.Format("{0}/{1}", CDN, message.ReceiverApplicationUser.Profile.UserImage.FileName),
                        ReceiverProfileId = message.ReceiverApplicationUser.Profile.Id
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                SentMessages = sentMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Sent
            };

            return View("index", viewModel);
        }
    }
}