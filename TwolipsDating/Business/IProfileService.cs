using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
namespace TwolipsDating.Business
{
    public interface IProfileService : IBaseService
    {
        System.Threading.Tasks.Task<int> AddTagSuggestionAsync(int tagId, int profileId, string suggestingUserId);
        System.Threading.Tasks.Task<UploadedImageServiceResult> AddUploadedImageForUserAsync(string userId, string fileName, bool isBanner = false);
        System.Threading.Tasks.Task<int> ChangeProfileBannerImageAsync(int profileId, int userImageId);
        System.Threading.Tasks.Task<ServiceResult> ChangeProfileUserImageAsync(int profileId, int userImageId);
        System.Threading.Tasks.Task<ServiceResult> CreateProfileAsync(int genderId, string cityName, string stateAbbreviation, string countryName, string userId, DateTime birthday);
        System.Threading.Tasks.Task<ServiceResult> DeleteBannerImageAsync(int userImageId);
        System.Threading.Tasks.Task<ServiceResult> DeleteUserImageAsync(int userImageId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.TagAndSuggestedCount>> GetAllTagsAndCountsAsync();
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Tag>> GetAllTagsAsync();
        System.Threading.Tasks.Task<int> GetGenderIdForProfileAsync(int profileId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Gender>> GetGendersAsync();
        System.Threading.Tasks.Task<int> GetGeoCityIdAsync(string cityName, string stateAbbreviation, string countryName);
        System.Threading.Tasks.Task<int> GetImagesUploadedCountByUserAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.InventoryItem>> GetInventoryAsync(string userId);
        System.Threading.Tasks.Task<int> GetInventoryCountAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Language>> GetLanguagesAsync();
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.LookingForLocation>> GetLookingForLocationsAsync();
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.LookingForType>> GetLookingForTypesAsync();
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.MessageConversation>> GetMessageConversationsAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Message>> GetMessagesBetweenUsersAsync(string userId, string userId2);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Message>> GetMessagesReceivedByUserAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Message>> GetMessagesSentByUserAsync(string userId);
        System.Threading.Tasks.Task<int> GetLifetimeForUserAsync(string userId);
        System.Threading.Tasks.Task<TwolipsDating.Models.Profile> GetProfileAsync(int profileId);
        System.Threading.Tasks.Task<TwolipsDating.Models.Profile> GetProfileAsync(string userId);
        System.Threading.Tasks.Task<int> GetPurchasedItemCountForUserAsync(string userId, int storeItemTypeId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetRandomProfilesForDashboardAsync(string currentUserId, int numberOfProfilesToRetrieve);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.RelationshipStatus>> GetRelationshipStatusesAsync();
        System.Threading.Tasks.Task<int> GetReviewsWrittenCountByUserAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Review>> GetReviewsWrittenForUserAsync(string targetUserId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Language>> GetSelectedLanguagesAsync(string currentUserId);
        System.Threading.Tasks.Task<int> GetSentGiftCountForUserAsync(string userId);
        System.Threading.Tasks.Task<int> GetTagAwardCountForUserAsync(string userId);
        System.Threading.Tasks.Task<int> GetTagCountAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.ProfileTagAwardViewModel>> GetTagsAwardedToProfileAsync(int profileId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.ProfileTagSuggestionViewModel>> GetTagsSuggestedForProfileAsync(string userId, int profileId);
        System.Threading.Tasks.Task<int> GetTagSuggestionCountForProfileAsync(int tagId, int profileId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.GiftTransactionLog>> GetUnreviewedGiftTransactionsAsync(string userId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.UserImage>> GetUserImagesAsync(string userId);
        System.Threading.Tasks.Task<TwolipsDating.ViewModels.UserStatsViewModel> GetUserStatsAsync(string userId);
        System.Threading.Tasks.Task<int> LogProfileViewAsync(string viewerUserId, int targetProfileId);
        System.Threading.Tasks.Task<int> RemoveAllGiftNotificationAsync(string userId);
        System.Threading.Tasks.Task<int> RemoveGiftNotificationAsync(string userId, int giftTransactionId);
        System.Threading.Tasks.Task<int> RemoveTagSuggestionAsync(int tagId, int profileId, string suggestingUserId);
        System.Threading.Tasks.Task<int> SendGiftAsync(string fromUserId, string toUserId, int giftId, int inventoryItemId, string senderProfileUrlRoot);
        System.Threading.Tasks.Task<ServiceResult> SendMessageAsync(string senderUserId, string receiverUserId, string body, string conversationUrl);
        System.Threading.Tasks.Task<int> SetBannerImagePositionAsync(int profileId, int bannerPositionX, int bannerPositionY);
        System.Threading.Tasks.Task<int> SetDetailsAsync(string userId, System.Collections.Generic.IReadOnlyCollection<int> languageIds, int? relationshipStatusId);
        System.Threading.Tasks.Task<int> SetLookingForAsync(string userId, int? lookingForTypeId, int? lookingForLocationId, int? lookingForAgeMin, int? lookingForAgeMax);
        System.Threading.Tasks.Task<int> SetSelectedTitle(string userId, int titleId);
        System.Threading.Tasks.Task<int> SetSelfSummaryAsync(string userId, string selfSummary);
        System.Threading.Tasks.Task<int> SetSummaryOfDoingAsync(string userId, string summaryOfDoing);
        System.Threading.Tasks.Task<int> SetSummaryOfGoingAsync(string userId, string summaryOfGoing);
        System.Threading.Tasks.Task<bool> ToggleFavoriteProfileAsync(string followerUserId, int followingProfileId, string profileIndexUrlRoot);
        System.Threading.Tasks.Task<ToggleServiceResult> ToggleIgnoredUserAsync(string sourceUserId, string targetUserId);
        IUserService UserService { set; }
        System.Threading.Tasks.Task<int> WriteReviewAsync(string authorUserId, string targetUserId, string content, int ratingValue, string authorProfileUrlRoot);

        System.Threading.Tasks.Task<bool> IsProfileFavoritedByUserAsync(int profileId, string currentUserId);
        Task<IReadOnlyCollection<SimilarUserViewModel>> GetSimilarProfilesAsync(int profileId);

        Task<IReadOnlyCollection<FollowerViewModel>> GetFollowersAsync(int profileId, string userId);
        Task<IReadOnlyCollection<FollowerViewModel>> GetFollowingAsync(int profileId, string userId);

        Task<int> GetFollowerCountAsync(int profileId);

        Task<int> GetFollowingCountAsync(int profileId);

        Task<Models.Profile> GetRandomProfileAsync(string currentUserId);

        Task<IReadOnlyCollection<GiftTransactionLog>> GetGiftsSentToUsersFromUserAsync(string userId, IEnumerable<string> userIds, TimeSpan duration);

        Task<IReadOnlyCollection<GiftTransactionLog>> GetGiftsSentToUserAsync(string userId, TimeSpan timeSpan);
    }
}
