﻿using AutoMapper;
using Microsoft.AspNet.Identity;
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
        #region Services

        private UserService userService = new UserService();

        #endregion Services

        #region Conversations

        /// <summary>
        /// Returns a view model containing conversations between the currently logged in user and another user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Conversation(string id)
        {
            // we want to look up conversations between a user and another user
            // if no id is provided, we want to lookup the latest conversation set
                // if there is no latest conversation set, we want to return a view that indicates to the user there are no messages
            // if id is provided, look up conversations for that id
                // if the id is invalid, return 404

            var currentUserId = User.Identity.GetUserId();

            // if the user doesn't have a profile, redirect them to the profile to create  it
            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            ConversationViewModel viewModel = new ConversationViewModel();

            // get recent conversations for the current user
            viewModel.Conversations = await GetRecentConversationsAsync(currentUserId);

            // setup notifications in the upper right
            await SetNotificationsAsync();

            // setup the essentials in the viewmodel
            viewModel.TargetApplicationUserId = id;
            viewModel.CurrentUserId = currentUserId;
            viewModel.IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

            // there are no recent conversations, return the view with an empty set
            if(viewModel.Conversations.Count == 0)
            {
                return View(viewModel);
            }

            // if no id value was provided for the conversation, look up the first one
            if (String.IsNullOrEmpty(id))
            {
                id = viewModel.Conversations[0].TargetUserId;
            }

            // lookup the profile we are accessing and the messages between the current user and that profile
            var profileForOtherUser = await ProfileService.GetUserProfileAsync(id);

            // there is no profile for the user that we are looking up
            if (profileForOtherUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // look up conversations between the current user and the selected id
            var messagesBetweenUsers = await ProfileService.GetMessagesBetweenUsersAsync(currentUserId, id);
            var conversationMessagesBetweenUsers = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyList<ConversationItemViewModel>>(messagesBetweenUsers);
            viewModel.ConversationMessages = conversationMessagesBetweenUsers;

            // setup targetted user for which conversations are being looked
            viewModel.TargetUserName = profileForOtherUser.ApplicationUser.UserName;
            viewModel.TargetUserAge = profileForOtherUser.Birthday.GetAge();
            viewModel.TargetUserLocation = profileForOtherUser.GeoCity.ToFullLocationString();
            viewModel.TargetProfileId = profileForOtherUser.Id;
            viewModel.TargetUserId = id;
            viewModel.TargetProfileImagePath = profileForOtherUser.GetProfileThumbnailImagePath();
            viewModel.TargetApplicationUserId = id;

            return View(viewModel);
        }

        /// <summary>
        /// Sets the viewmodel up with the recent conversations that have taken place between the passed current user id
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<ConversationItemViewModel>> GetRecentConversationsAsync(string currentUserId)
        {
            // get all messages sent and received by the current user
            var messagesForUser = await ProfileService.GetMessageConversationsAsync(currentUserId);

            // setup the viewmodel to only include the most recent message from a conversation (like google hangouts home screen)
            Dictionary<ConversationKey, ConversationItemViewModel> conversations = new Dictionary<ConversationKey, ConversationItemViewModel>();
            foreach (var message in messagesForUser)
            {
                ConversationItemViewModel conversation = GetConversationItemViewModel(currentUserId, message);

                // each conversation is considered unique by the participants
                // if participant A messages B and B messages A, both messages are part of the same conversation because the participants are the same
                ConversationKey conversationKey = new ConversationKey()
                {
                    ParticipantUserId1 = message.SenderApplicationUserId,
                    ParticipantUserId2 = message.ReceiverApplicationUserId
                };

                UpdateConversationCollection(conversations, conversation, conversationKey);
            }

            return conversations.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Updates the collection of conversations to include the passed conversation if it doesn't already exist in the collection. If the passed conversation item is more
        /// recent than the existing conversation item in the collection, the existing item is replaced. We do this so that we can display the most recent item of a conversation to the user.
        /// </summary>
        /// <param name="conversations"></param>
        /// <param name="conversation"></param>
        /// <param name="conversationHolder"></param>
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

        /// <summary>
        /// Returns a single conversation item view model based on the backing conversation model.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private ConversationItemViewModel GetConversationItemViewModel(string currentUserId, MessageConversation message)
        {
            ConversationItemViewModel conversationItem = Mapper.Map<MessageConversation, ConversationItemViewModel>(message);

            if (message.ReceiverApplicationUserId == currentUserId)
            {
                conversationItem.TargetUserId = message.SenderApplicationUserId;
                conversationItem.TargetName = message.SenderName;
                conversationItem.TargetProfileImagePath = message.GetSenderProfileImagePath();
                conversationItem.TargetProfileId = message.SenderProfileId;
            }
            else if (message.SenderApplicationUserId == currentUserId)
            {
                conversationItem.TargetUserId = message.ReceiverApplicationUserId;
                conversationItem.TargetName = message.ReceiverName;
                conversationItem.TargetProfileImagePath = message.GetReceiverProfileImagePath();
                conversationItem.TargetProfileId = message.ReceiverProfileId;
            }

            if (message.SenderApplicationUserId == currentUserId)
            {
                conversationItem.MostRecentMessageBody = String.Format("You: {0}", conversationItem.MostRecentMessageBody);
            }

            return conversationItem;
        }

        #endregion Conversations

        #region Sent/Received

        /// <summary>
        /// Sets up a view model displaying all received messages for the currently logged in user.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Received()
        {
            var currentUserId = User.Identity.GetUserId();

            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            var messages = await ProfileService.GetMessagesReceivedByUserAsync(currentUserId);

            var receivedMessages = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyCollection<ReceivedMessageViewModel>>(messages);

            MessageViewModel viewModel = new MessageViewModel()
            {
                ReceivedMessages = receivedMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Received,
                IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId)
            };

            await SetNotificationsAsync();

            return View("index", viewModel);
        }

        /// <summary>
        /// Sets up a view model displaying all sent messages for the currently logged in user.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Sent()
        {
            var currentUserId = User.Identity.GetUserId();

            if (!(await userService.DoesUserHaveProfileAsync(currentUserId))) return RedirectToProfileIndex();

            var messages = await ProfileService.GetMessagesReceivedByUserAsync(currentUserId);

            var sentMessages = Mapper.Map<IReadOnlyCollection<Message>, IReadOnlyCollection<SentMessageViewModel>>(messages);

            MessageViewModel viewModel = new MessageViewModel()
            {
                SentMessages = sentMessages.OrderByDescending(m => m.DateSent).ToList().AsReadOnly(),
                MessageViewMode = MessageViewMode.Sent,
                IsCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId)
            };

            await SetNotificationsAsync();

            return View("index", viewModel);
        }

        /// <summary>
        /// Redirects to the conversation action.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        protected ActionResult RedirectToConversation(object routeValues = null)
        {
            return RedirectToAction("conversation", routeValues);
        }

        /// <summary>
        /// Sends a message to a user from the currently logged in user. Does nothing if the current user's email address isn't confirmed.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Send(ConversationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToConversation(new { id = viewModel.TargetApplicationUserId });
            }

            try
            {
                string currentUserId = User.Identity.GetUserId();

                bool isCurrentUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(currentUserId);

                if (isCurrentUserEmailConfirmed)
                {
                    string conversationUrl = Url.ActionWithFullUrl(Request, "conversation", "message", new { id = currentUserId });
                    int changes = await ProfileService.SendMessageAsync(currentUserId, viewModel.TargetApplicationUserId, viewModel.NewMessage, conversationUrl);

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

        #endregion Sent/Received

        /// <summary>
        /// Disposes all services.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (userService != null)
                {
                    userService.Dispose();
                    userService = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}