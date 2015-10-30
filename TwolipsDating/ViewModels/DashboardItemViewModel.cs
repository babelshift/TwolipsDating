using System;

namespace TwolipsDating.ViewModels
{
    public enum DashboardFeedItemType
    {
        Message,
        UploadedPictures,
        ReviewWritten,
        GiftTransaction,
        TagSuggestion,
        QuizCompletion,
        AchievementObtained,
        NewFollower,
        ProfileVisit
    }

    public class DashboardItemViewModel
    {
        public DateTime DateOccurred { get; set; }
        public DashboardFeedItemType ItemType { get; set; }
        public MessageFeedViewModel MessageFeedItem { get; set; }
        public UploadedImageFeedViewModel UploadedImageFeedItem { get; set; }
        public ReviewWrittenFeedViewModel ReviewWrittenFeedItem { get; set; }
        public GiftReceivedFeedViewModel GiftReceivedFeedItem { get; set; }
        public TagSuggestionReceivedFeedViewModel TagSuggestionReceivedFeedItem { get; set; }
        public CompletedQuizFeedViewModel CompletedQuizFeedItem { get; set; }
        public AchievementFeedViewModel AchievementFeedItem { get; set; }
        public FollowerFeedViewModel FollowerFeedItem { get; set; }
        public ProfileVisitFeedViewModel ProfileVisitFeedItem { get; set; }
    }
}