using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ProfileViewModel
    {
        public enum ProfileViewMode
        {
            ShowProfile,
            CreateProfile,
            NoProfile
        }

        #region Profile view stuff

        public bool IsCurrentUserEmailConfirmed { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public int ProfileId { get; set; }
        public string ProfileUserId { get; set; }
        public string ActiveTab { get; set; }
        public int AverageRatingValue { get; set; }
        public string ProfileImagePath { get; set; }
        public string CurrentUserId { get; set; }
        public ProfileViewMode ViewMode { get; set; }

        public ProfileFeedViewModel Feed { get; set; }
        public ProfileReviewsViewModel Reviews { get; set; }
        public ProfileInventoryViewModel Inventory { get; set; }
        public IReadOnlyCollection<InventoryItemViewModel> ViewerInventoryItems { get; set; }
        public IReadOnlyCollection<ProfileTagSuggestionViewModel> SuggestedTags { get; set; }
        public IReadOnlyCollection<ProfileTagSuggestionViewModel> AllTags { get; set; }

        public bool IsIgnoredByCurrentUser { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; }

        #endregion

        #region Write review stuff

        public WriteReviewViewModel WriteReview { get; set; }

        #endregion

        #region Send message stuff

        public SendMessageViewModel SendMessage { get; set; }

        #endregion

        #region Profile creation stuff

        public CreateProfileViewModel CreateProfile { get; set; }

        #endregion

        #region Image upload stuff

        public UploadImageViewModel UploadImage { get; set; }

        #endregion

        #region Send gift stuff

        public SendGiftViewModel SendGift { get; set; }

        #endregion

        #region Profile image stuff

        public ChangeProfileImageViewModel ChangeImage { get; set; }

        #endregion
    }
}