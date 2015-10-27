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

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.MilestoneAchievement>> GetRecentFollowerAchievementsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.GiftReceivedFeedViewModel>> GetRecentFollowerGiftTransactionsAsync(string userId, FeedItemQueryType queryType);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.UploadedImageFeedViewModel>> GetRecentFollowerImagesAsync(string userId);

        Task<IReadOnlyCollection<UploadedImageFeedViewModel>> GetImagesForUserFeedAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.CompletedQuizFeedViewModel>> GetRecentFollowerQuizCompletionsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.ReviewWrittenFeedViewModel>> GetRecentReviewsFeedAsync(string userId, FeedItemQueryType queryType);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.FavoriteProfile>> GetRecentFollowersAsync(string currentUserId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.TagSuggestion>> GetRecentFollowerTagSuggestionsAsync(string userId);
    }
}