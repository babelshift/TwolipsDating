using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class MessageController : BaseController
    {
        ProfileService p = new ProfileService();

        public async Task<ActionResult> Received()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            SetUnreadCountsInViewBag(p, user);

            var messages = p.GetMessagesByUser(user.Id);

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
                        Subject = message.Subject
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                ReceivedMessages = receivedMessages,
                MessageViewMode = MessageViewMode.Received
            };

            return View("index", viewModel);
        }

        public async Task<ActionResult> Sent()
        {

            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            SetUnreadCountsInViewBag(p, user);

            var messages = p.GetMessagesByUser(user.Id);

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
                        Subject = message.Subject
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                SentMessages = sentMessages,
                MessageViewMode = MessageViewMode.Sent
            };

            return View("index", viewModel);
        }
    }
}