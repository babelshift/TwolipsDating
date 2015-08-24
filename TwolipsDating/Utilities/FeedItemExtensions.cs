using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Utilities
{
    public static class FeedItemExtensions
    {
        public static IReadOnlyCollection<TagSuggestionReceivedFeedViewModel> GetConsolidatedTagsSuggested(this IReadOnlyCollection<TagSuggestion> tagSuggestions)
        {
            // translates the collection of entities into the view models that we want to work with
            var tagSuggestionViewModels = Mapper.Map<IReadOnlyCollection<TagSuggestion>, IReadOnlyCollection<TagSuggestionReceivedFeedViewModel>>(tagSuggestions);

            // a dictionary containing the unique transactions
            var consolidatedTagSuggestionViewModels = new Dictionary<TagSuggestionFeedItemKey, TagSuggestionReceivedFeedViewModel>();

            // for every transaction, loop and consolidate
            foreach (var tagSuggestionViewModel in tagSuggestionViewModels)
            {
                UpdateTagSuggestionCollection(consolidatedTagSuggestionViewModels, tagSuggestionViewModel);
            }

            return consolidatedTagSuggestionViewModels.Values.ToList().AsReadOnly();
        }

        private static void UpdateTagSuggestionCollection(Dictionary<TagSuggestionFeedItemKey, TagSuggestionReceivedFeedViewModel> consolidatedTagSuggestionViewModels, 
            TagSuggestionReceivedFeedViewModel tagSuggestionFeed)
        {
            // the key to identify if a transaction is unique or not depends on the user id and the time since the transaction occurred
            TagSuggestionFeedItemKey feedKey = new TagSuggestionFeedItemKey()
            {
                UserId = tagSuggestionFeed.SuggestUserId,
                TimeAgo = tagSuggestionFeed.TimeAgo,
                ProfileId = tagSuggestionFeed.ReceiverProfileId
            };

            TagSuggestionReceivedFeedViewModel existingTagSuggestionFeed = new TagSuggestionReceivedFeedViewModel();

            // check if the transaction for this user and time is already accounted for
            bool tagSuggestionAlreadyInCollection = consolidatedTagSuggestionViewModels.TryGetValue(feedKey, out existingTagSuggestionFeed);

            // if it isn't, then add it to our consolidated collection to begin consolidating
            if (!tagSuggestionAlreadyInCollection)
            {
                consolidatedTagSuggestionViewModels.Add(feedKey, tagSuggestionFeed);
            }
            // if it is, then add the image path to the consolidated collection
            else
            {
                if (tagSuggestionFeed.Tags.Count > 0)
                {
                    existingTagSuggestionFeed.Tags.Add(tagSuggestionFeed.Tags[0]);
                }
            }
        }

        /// <summary>
        /// On the user's feed, we want to consolidate multiple "Sent gift" notices into a single line item. For example, the following transactions should be consolidated:
        /// "jskiles sent a 'Red Rose' gift to justin on Aug 10"
        /// "jskiles sent a 'Red Rose' gift to justin on Aug 10"
        /// "jskiles sent a 'Red Rose' gift to justin on Aug 10"
        /// "jskiles sent a 'White Rose' gift to justin on Aug 10"
        /// 
        /// Further, all gifts of the same ID should be summed into a single count. The above would condense to a single line of:
        /// "jskiles sent 3x 'Red Rose' gifts and 1x 'White Rose' gift to justin on Aug 10"
        /// </summary>
        /// <param name="giftTransactions"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<GiftReceivedFeedViewModel> GetConsolidatedGiftTransactions(this IReadOnlyCollection<GiftTransactionLog> giftTransactions)
        {
            Dictionary<FeedItemKey, GiftReceivedFeedViewModel> consolidatedGiftTransactionViewModels = new Dictionary<FeedItemKey, GiftReceivedFeedViewModel>();

            foreach (var giftTransaction in giftTransactions)
            {
                // the key to identify if a transaction is unique or not depends on the user id and the time since the transaction occurred
                FeedItemKey key = GetFeedItemKey(giftTransaction);

                GiftReceivedFeedViewModel existingGiftReceivedFeedViewModel = new GiftReceivedFeedViewModel();
                bool alreadyExists = consolidatedGiftTransactionViewModels.TryGetValue(key, out existingGiftReceivedFeedViewModel);
                if (!alreadyExists)
                {
                    var newGiftReceivedFeedViewModel = Mapper.Map<GiftTransactionLog, GiftReceivedFeedViewModel>(giftTransaction);
                    AddGiftReceivedFeedItem(newGiftReceivedFeedViewModel, giftTransaction);
                    consolidatedGiftTransactionViewModels.Add(key, newGiftReceivedFeedViewModel);
                }
                else
                {
                    GiftReceivedFeedItemViewModel existingGiftReceivedFeedItemViewModel = new GiftReceivedFeedItemViewModel();
                    alreadyExists = existingGiftReceivedFeedViewModel.Gifts.TryGetValue(giftTransaction.StoreItemId, out existingGiftReceivedFeedItemViewModel);
                    if (!alreadyExists)
                    {
                        AddGiftReceivedFeedItem(existingGiftReceivedFeedViewModel, giftTransaction);
                    }
                    else
                    {
                        existingGiftReceivedFeedItemViewModel.GiftSentCount++;
                    }
                }
            }

            return consolidatedGiftTransactionViewModels.Values.ToList().AsReadOnly();
        }

        private static void AddGiftReceivedFeedItem(GiftReceivedFeedViewModel giftReceivedFeedViewModel, GiftTransactionLog giftTransaction)
        {
            giftReceivedFeedViewModel.Gifts.Add(giftTransaction.StoreItemId, new GiftReceivedFeedItemViewModel()
            {
                GiftImagePath = giftTransaction.StoreItem.GetIconPath(),
                GiftSentCount = giftTransaction.ItemCount
            });
        }

        private static FeedItemKey GetFeedItemKey(GiftTransactionLog giftTransaction)
        {
            FeedItemKey key = new FeedItemKey()
            {
                UserId = giftTransaction.FromUserId,
                TimeAgo = giftTransaction.DateTransactionOccurred.GetTimeAgo()
            };
            return key;
        }

        /// <summary>
        /// On the user's feed, we want to consolidate multiple "Uploaded image" notices into a single line item. For example, the following transactions should be consolidated:
        /// "jskiles uploaded an image 123.jpg on Aug 10"
        /// "jskiles uploaded an image 345.jpg on Aug 10"
        /// </summary>
        /// <param name="giftTransactions"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<UploadedImageFeedViewModel> GetConsolidatedImages(this IReadOnlyCollection<UserImage> uploadedImages)
        {
            // translates the collection of entities into the view models that we want to work with
            var uploadedImageViewModels = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UploadedImageFeedViewModel>>(uploadedImages);

            // a dictionary containing the unique transactions
            Dictionary<FeedItemKey, UploadedImageFeedViewModel> consolidatedUploadedImageViewModels = new Dictionary<FeedItemKey, UploadedImageFeedViewModel>();

            // for every transaction, loop and consolidate
            foreach (var uploadedImageViewModel in uploadedImageViewModels)
            {
                UpdateUserImageCollection(consolidatedUploadedImageViewModels, uploadedImageViewModel);
            }

            return consolidatedUploadedImageViewModels.Values.ToList().AsReadOnly();
        }

        private static void UpdateUserImageCollection(Dictionary<FeedItemKey, UploadedImageFeedViewModel> userImagesViewModelConsolidated, UploadedImageFeedViewModel uploadedImageFeed)
        {
            // the key to identify if a transaction is unique or not depends on the user id and the time since the transaction occurred
            FeedItemKey feedKey = new FeedItemKey()
            {
                UserId = uploadedImageFeed.UploaderUserId,
                TimeAgo = uploadedImageFeed.TimeAgo
            };

            UploadedImageFeedViewModel existingUploadedImageFeed = new UploadedImageFeedViewModel();

            // check if the transaction for this user and time is already accounted for
            bool uploadedImageAlreadyInCollection = userImagesViewModelConsolidated.TryGetValue(feedKey, out existingUploadedImageFeed);

            // if it isn't, then add it to our consolidated collection to begin consolidating
            if (!uploadedImageAlreadyInCollection)
            {
                userImagesViewModelConsolidated.Add(feedKey, uploadedImageFeed);
            }
            // if it is, then add the image path to the consolidated collection
            else
            {
                if (uploadedImageFeed.UploadedImagesPaths.Count > 0)
                {
                    existingUploadedImageFeed.UploadedImagesPaths.Add(uploadedImageFeed.UploadedImagesPaths[0]);
                }
            }
        }
    }
}