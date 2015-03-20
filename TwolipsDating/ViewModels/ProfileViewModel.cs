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

        public IReadOnlyCollection<ProfileFeedViewModel> Feed { get; set; }
        public IReadOnlyCollection<ReviewViewModel> Reviews { get; set; }
        public IReadOnlyCollection<ProfileTagSuggestionViewModel> Tags { get; set; }

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

        #region Profile image stuff

        public ChangeProfileImageViewModel ChangeImage { get; set; }

        #endregion
    }
}