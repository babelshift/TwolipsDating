using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class MessageController : BaseController
    {
        #region Conversations

        public async Task<ActionResult> Conversation(string id)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            ConversationViewModel viewModel = new ConversationViewModel();

            // if id has a value, look up all messages between the current user and the passed user id
            if (!String.IsNullOrEmpty(id))
            {
                viewModel.TargetApplicationUserId = id;

                // lookup the profile we are accessing and the messages between the current user and that profile
                var profileForOtherUser = await ProfileService.GetUserProfileAsync(id);

                if (profileForOtherUser == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                var messagesBetweenUsers = await ProfileService.GetMessagesBetweenUsersAsync(currentUserId, id);

                // setup the conversation view model
                var conversationMessages = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyList<ConversationItemViewModel>>(messagesBetweenUsers);

                viewModel.ConversationMessages = conversationMessages;
                viewModel.TargetUserName = profileForOtherUser.ApplicationUser.UserName;
                viewModel.TargetUserAge = profileForOtherUser.Birthday.GetAge();
                viewModel.TargetUserLocation = profileForOtherUser.City.GetCityAndState();
                viewModel.TargetProfileId = profileForOtherUser.Id;

                // if the profile we are looking up has a profile image, set the url it appropriately
                if (profileForOtherUser.UserImage != null && !String.IsNullOrEmpty(profileForOtherUser.UserImage.FileName))
                {
                    viewModel.TargetProfileImagePath = profileForOtherUser.GetProfileImagePath();
                }

                viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                await SetUnreadCountsInViewBagAsync();

                return View(viewModel);
            }
            // otherwise, look up the most recent messages and conversations for the current user
            else
            {
                viewModel.TargetApplicationUserId = currentUserId;

                // get all messages sent and received by the current user
                var messagesForUser = await ProfileService.GetMessageConversationsAsync(currentUserId);

                // setup the viewmodel to only include the most recent message from a conversation (like google hangouts home screen)
                Dictionary<ConversationKey, ConversationItemViewModel> conversations = new Dictionary<ConversationKey, ConversationItemViewModel>();
                foreach (var message in messagesForUser)
                {
                    ConversationItemViewModel conversation = GetConversationViewModel(currentUserId, message);

                    ConversationKey conversationKey = new ConversationKey()
                    {
                        ParticipantUserId1 = message.SenderApplicationUserId,
                        ParticipantUserId2 = message.ReceiverApplicationUserId
                    };

                    UpdateConversationCollection(conversations, conversation, conversationKey);
                }

                viewModel.Conversations = conversations.Values.ToList().AsReadOnly();

                viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                await SetUnreadCountsInViewBagAsync();

                return View(viewModel);
            }
        }

        private static void UpdateConversationCollection(Dictionary<ConversationKey, ConversationItemViewModel> conversations, ConversationItemViewModel conversation, ConversationKey conversationHolder)
        {
            ConversationItemViewModel existingConversation = new ConversationItemViewModel();
            bool alreadyInCollection = conversations.TryGetValue(conversationHolder, out existingConversation);
            if (!alreadyInCollection)
            {
                conversations.Add(conversationHolder, conversation);
            }
            else
            {
                // if the iterating message is more recent than the existing message for this conversation, remove existing, add new
                if (conversation.DateSent > existingConversation.DateSent)
                {
                    conversations[conversationHolder] = conversation;
                }
            }
        }

        private ConversationItemViewModel GetConversationViewModel(string currentUserId, MessageConversation message)
        {
            ConversationItemViewModel conversation = Mapper.Map<MessageConversation, ConversationItemViewModel>(message);

            if (message.ReceiverApplicationUserId == currentUserId)
            {
                conversation.TargetUserId = message.SenderApplicationUserId;
                conversation.TargetName = message.SenderName;
                conversation.TargetProfileImagePath = message.GetSenderProfileImagePath();
                conversation.TargetProfileId = message.SenderProfileId;
            }
            else if (message.SenderApplicationUserId == currentUserId)
            {
                conversation.TargetUserId = message.ReceiverApplicationUserId;
                conversation.TargetName = message.ReceiverName;
                conversation.TargetProfileImagePath = message.GetReceiverProfileImagePath();
                conversation.TargetProfileId = message.ReceiverProfileId;
            }

            if (message.SenderApplicationUserId == currentUserId)
            {
                conversation.MostRecentMessageBody = String.Format("You: {0}", conversation.MostRecentMessageBody);
            }

            return conversation;
        }

        #endregion

        #region Sent/Received

        public async Task<ActionResult> Received()
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var messages = await ProfileService.GetMessagesReceivedByUserAsync(currentUserId);

            var receivedMessages = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyCollection<ReceivedMessageViewModel>>(messages);

            MessageViewModel viewModel = new MessageViewModel()
            {
                ReceivedMessages = receivedMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Received,
                IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId)
            };

            await SetUnreadCountsInViewBagAsync();

            return View("index", viewModel);
        }

        public async Task<ActionResult> Sent()
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var messages = await ProfileService.GetMessagesByUserAsync(currentUserId);

            var sentMessages = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyCollection<SentMessageViewModel>>(messages);

            MessageViewModel viewModel = new MessageViewModel()
            {
                SentMessages = sentMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Sent,
                IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId)
            };

            await SetUnreadCountsInViewBagAsync();

            return View("index", viewModel);
        }


        protected ActionResult RedirectToConversation(object routeValues = null)
        {
            return RedirectToAction("conversation", routeValues);
        }


        [HttpPost]
        public async Task<ActionResult> Send(ConversationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToConversation(new { id = viewModel.TargetApplicationUserId });
            }

            try
            {
                string currentUserId = await GetCurrentUserIdAsync();

                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    int changes = await ProfileService.SendMessageAsync(currentUserId, viewModel.TargetApplicationUserId, viewModel.NewMessage);

                    if (changes == 0)
                    {
                        Log.Warn(
                            "SendMessage",
                            ErrorMessages.MessageNotSent,
                            new { targetProfileId = viewModel.TargetProfileId, newMessage = viewModel.NewMessage }
                        );

                        AddError(ErrorMessages.MessageNotSent);
                    }
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "SendMessage",
                    e,
                            new { targetProfileId = viewModel.TargetProfileId, newMessage = viewModel.NewMessage }
                );

                AddError(ErrorMessages.MessageNotSent);
            }

            return RedirectToConversation(new { id = viewModel.TargetApplicationUserId });
        }

        #endregion
    }
}