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
using AutoMapper;

namespace TwolipsDating.Controllers
{
    public class MessageController : BaseController
    {
        #region Conversations

        public async Task<ActionResult> Conversation(string id)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            // if id has a value, look up all messages between the current user and the passed user id
            if (!String.IsNullOrEmpty(id))
            {
                // lookup the profile we are accessing and the messages between the current user and that profile
                var profileForOtherUser = await ProfileService.GetUserProfileAsync(id);
                var messagesBetweenUsers = await ProfileService.GetMessagesBetweenUsersAsync(currentUserId, id);

                // setup the conversation view model
                var conversationMessages = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyList<ConversationItemViewModel>>(messagesBetweenUsers);
                ConversationViewModel viewModel = new ConversationViewModel()
                {
                    ConversationMessages = conversationMessages,
                    TargetUserName = profileForOtherUser.ApplicationUser.UserName,
                    TargetUserAge = profileForOtherUser.Birthday.GetAge(),
                    TargetUserLocation = profileForOtherUser.City.GetCityAndState(),
                    TargetProfileId = profileForOtherUser.Id
                };

                // if the profile we are looking up has a profile image, set the url it appropriately
                if(profileForOtherUser.UserImage != null && !String.IsNullOrEmpty(profileForOtherUser.UserImage.FileName))
                {
                    viewModel.TargetProfileImagePath = profileForOtherUser.GetProfileImagePath();
                }

                viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                await SetUnreadCountsInViewBag();

                return View(viewModel);
            }
            // otherwise, look up the most recent messages and conversations for the current user
            else
            {
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

                ConversationViewModel viewModel = new ConversationViewModel();
                viewModel.Conversations = conversations.Values.ToList().AsReadOnly();

                viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                await SetUnreadCountsInViewBag();

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
                conversation.TargetProfileImagePath = !String.IsNullOrEmpty(message.SenderProfileImageFileName) ? String.Format("{0}/{1}", CDN, message.SenderProfileImageFileName) : String.Empty;
                conversation.TargetProfileId = message.SenderProfileId;
            }
            else if (message.SenderApplicationUserId == currentUserId)
            {
                conversation.TargetUserId = message.ReceiverApplicationUserId;
                conversation.TargetName = message.ReceiverName;
                conversation.TargetProfileImagePath = !String.IsNullOrEmpty(message.ReceiverProfileImageFileName) ? String.Format("{0}/{1}", CDN, message.ReceiverProfileImageFileName) : String.Empty;
                conversation.TargetProfileId = message.ReceiverProfileId;
            }

            if(message.SenderApplicationUserId == currentUserId)
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

            var messages = await ProfileService.GetMessagesByUserAsync(currentUserId);

            List<ReceivedMessageViewModel> receivedMessages = new List<ReceivedMessageViewModel>();
            foreach (var message in messages)
            {
                if (message.ReceiverApplicationUserId == currentUserId)
                {
                    receivedMessages.Add(new ReceivedMessageViewModel()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        DateSent = message.DateSent,
                        SenderName = message.SenderApplicationUser.UserName,
                        TimeAgo = message.DateSent.GetTimeAgo(),
                        SenderProfileImagePath = message.SenderApplicationUser.Profile.GetProfileImagePath(),
                        SenderProfileId = message.SenderApplicationUser.Profile.Id
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                ReceivedMessages = receivedMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Received,
                IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId)
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
                    sentMessages.Add(new SentMessageViewModel()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        DateSent = message.DateSent,
                        ReceiverName = message.ReceiverApplicationUser.UserName,
                        TimeAgo = message.DateSent.GetTimeAgo(),
                        ReceiverProfileImagePath = message.ReceiverApplicationUser.Profile.GetProfileImagePath(),
                        ReceiverProfileId = message.ReceiverApplicationUser.Profile.Id
                    });
                }
            }

            MessageViewModel viewModel = new MessageViewModel()
            {
                SentMessages = sentMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Sent,
                IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId)
            };

            await SetUnreadCountsInViewBag();

            return View("index", viewModel);
        }

        #endregion
    }
}