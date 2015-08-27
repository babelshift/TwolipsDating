using System;

namespace TwolipsDating.ViewModels
{
    public enum ProfileFeedItemType
    {
        UploadedPictures,
        ReviewWritten
    }

    public class ProfileFeedItemViewModel
    {
        public DateTime DateOccurred { get; set; }
        public DashboardFeedItemType ItemType { get; set; }
        public UploadedImageFeedViewModel UploadedImageFeedItem { get; set; }
        public ReviewWrittenFeedViewModel ReviewWrittenFeedItem { get; set; }
        public GiftReceivedFeedViewModel GiftReceivedFeedItem { get; set; }
        public TagSuggestionReceivedFeedViewModel TagSuggestionReceivedFeedItem { get; set; }
        public CompletedQuizFeedViewModel CompletedQuizFeedItem { get; set; }
        public AchievementFeedViewModel AchievementFeedItem { get; set; }
    }
}