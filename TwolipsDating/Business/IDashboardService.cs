using System.Collections.Generic;
using System.Threading.Tasks;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public interface IDashboardService : IBaseService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.MilestoneAchievement>> GetFollowerAchievementsForUserAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.TagSuggestion>> GetFollowerTagSuggestionsForUserAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.GiftReceivedFeedViewModel>> GetGiftTransactionsForUserFeedAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.CompletedQuizFeedViewModel>> GetQuizCompletionsForUserAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.MilestoneAchievement>> GetFollowerAchievementsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.GiftReceivedFeedViewModel>> GetFollowerGiftTransactionsAsync(string userId, FeedItemQueryType queryType);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.UploadedImageFeedViewModel>> GetFollowerUploadedImagesAsync(string userId);

        Task<IReadOnlyCollection<UploadedImageFeedViewModel>> GetUploadedImagesForUserFeedAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.CompletedQuizFeedViewModel>> GetFollowerQuizCompletionsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.ReviewWrittenFeedViewModel>> GetReviewsForFeedAsync(string userId, FeedItemQueryType queryType);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.FavoriteProfile>> GetFollowersAsync(string currentUserId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.TagSuggestion>> GetFollowerTagSuggestionsAsync(string userId);
    }
}