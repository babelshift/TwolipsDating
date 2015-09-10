namespace TwolipsDating.Business
{
    public interface IDashboardService : IBaseService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.MilestoneAchievement>> GetFollowerAchievementsForUserAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.TagSuggestion>> GetFollowerTagSuggestionsForUserAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.GiftTransactionLog>> GetGiftTransactionsForUserAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.CompletedQuizFeedViewModel>> GetQuizCompletionsForUserAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.MilestoneAchievement>> GetRecentFollowerAchievementsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.GiftTransactionLog>> GetRecentFollowerGiftTransactionsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.UserImage>> GetRecentFollowerImagesAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.CompletedQuizFeedViewModel>> GetRecentFollowerQuizCompletionsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Review>> GetRecentFollowerReviewsAsync(string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.FavoriteProfile>> GetRecentFollowersAsync(string currentUserId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.TagSuggestion>> GetRecentFollowerTagSuggestionsAsync(string userId);
    }
}