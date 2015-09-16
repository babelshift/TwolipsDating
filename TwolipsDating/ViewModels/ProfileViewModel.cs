using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TwolipsDating.Models;

namespace TwolipsDating.ViewModels
{
    public class ProfileViewModel
    {
        #region Profile view stuff

        public bool IsCurrentUserEmailConfirmed { get; set; }
        public string UserName { get; set; }
        public string SelectedTitle { get; set; }
        public string SelectedTitleImage { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public int ProfileId { get; set; }
        public string ProfileUserId { get; set; }
        public string ActiveTab { get; set; }
        public int AverageRatingValue { get; set; }
        public string ProfileThumbnailImagePath { get; set; }
        public string ProfileImagePath { get; set; }
        public int ReviewCount { get; set; }

        public ProfileFeedViewModel Feed { get; set; }
        public ProfileReviewsViewModel Reviews { get; set; }
        public ProfileInventoryViewModel Inventory { get; set; }
        public IReadOnlyCollection<InventoryItemViewModel> ViewerInventoryItems { get; set; }
        public IReadOnlyCollection<ProfileTagSuggestionViewModel> SuggestedTags { get; set; }
        public IReadOnlyCollection<ProfileTagSuggestionViewModel> AllTags { get; set; }
        public IReadOnlyCollection<ProfileTagAwardViewModel> AwardedTags { get; set; }
        public IReadOnlyCollection<TitleViewModel> UserTitles { get; set; }
        public IReadOnlyCollection<AchievementOverviewViewModel> Achievements { get; set; }
        public IReadOnlyCollection<SimilarUserViewModel> SimilarUsers { get; set; }

        public bool IsIgnoredByCurrentUser { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; }

        public int FeedCount { get; set; }
        public int PictureCount { get; set; }
        public int TagCount { get; set; }
        public int InventoryCount { get; set; }
        public int CompletedAchievementCount { get; set; }
        public int PossibleAchievementCount { get; set; }

        [StringLength(500)]
        public string SummaryOfSelf { get; set; }

        [StringLength(500)]
        public string SummaryOfDoing { get; set; }

        [StringLength(500)]
        public string SummaryOfGoing { get; set; }

        public string LookingForType { get; set; }
        [Display(Name="What type of relationship are you looking for, if any?")]
        public int? LookingForTypeId { get; set; }
        public IDictionary<int, string> LookingForTypes { get; set; }

        public string RelationshipStatus { get; set; }
        [Display(Name = "What is your current relationship status, if any?")]
        public int? RelationshipStatusId { get; set; }
        public IDictionary<int, string> RelationshipStatuses { get; set; }

        public string LookingForLocation { get; set; }
        [Display(Name = "How far away are you willing to look?")]
        public int? LookingForLocationId { get; set; }
        public IDictionary<int, string> LookingForLocations { get; set; }

        public IList<string> Languages { get; set; }
        [Display(Name = "What languages do you use?")]
        public IList<int> SelectedLanguages { get; set; }
        public IDictionary<int, string> AllLanguages { get; set; }

        public int? LookingForAgeMin { get; set; }
        public int? LookingForAgeMax { get; set; }

        public string LastLoginTimeAgo { get; set; }

        public string BannerImagePath { get; set; }
        public int BannerPositionX { get; set; }
        public int BannerPositionY { get; set; }

        public int LifeTimePoints { get; set; }

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