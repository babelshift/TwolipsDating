using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class ProfileViewModel
    {
        #region Profile view stuff

        public bool IsCurrentUserEmailConfirmed { get; set; }
        public string UserName { get; set; }
        public string SelectedTitle { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public int ProfileId { get; set; }
        public string ProfileUserId { get; set; }
        public string ActiveTab { get; set; }
        public int AverageRatingValue { get; set; }
        public string ProfileThumbnailImagePath { get; set; }
        public string ProfileImagePath { get; set; }
        public string CurrentUserId { get; set; }
        public int ReviewCount { get; set; }
        public string SelfDescription { get; set; }

        public ProfileFeedViewModel Feed { get; set; }
        public ProfileReviewsViewModel Reviews { get; set; }
        public ProfileInventoryViewModel Inventory { get; set; }
        public IReadOnlyCollection<InventoryItemViewModel> ViewerInventoryItems { get; set; }
        public IReadOnlyCollection<ProfileTagSuggestionViewModel> SuggestedTags { get; set; }
        public IReadOnlyCollection<ProfileTagSuggestionViewModel> AllTags { get; set; }
        public IReadOnlyCollection<ProfileTagAwardViewModel> AwardedTags { get; set; }
        public IReadOnlyCollection<TitleViewModel> UserTitles { get; set; }
        public IReadOnlyCollection<AchievementOverviewViewModel> Achievements { get; set; }

        public bool IsIgnoredByCurrentUser { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; }

        #endregion Profile view stuff

        #region Report review stuff

        public WriteReviewViolationViewModel WriteReviewViolation { get; set; }

        #endregion Report review stuff

        #region Write review stuff

        public WriteReviewViewModel WriteReview { get; set; }

        #endregion Write review stuff

        #region Send message stuff

        public SendMessageViewModel SendMessage { get; set; }

        #endregion Send message stuff

        #region Image upload stuff

        public UploadImageViewModel UploadImage { get; set; }

        #endregion Image upload stuff

        #region Send gift stuff

        public SendGiftViewModel SendGift { get; set; }

        #endregion Send gift stuff

        #region Profile image stuff

        public ChangeProfileImageViewModel ChangeImage { get; set; }

        #endregion Profile image stuff
    }
}