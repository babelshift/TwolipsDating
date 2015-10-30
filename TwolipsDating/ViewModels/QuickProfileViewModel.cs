using System;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class QuickProfileViewModel
    {
        public string ProfileUserId { get; set; }
        public string BannerImagePath { get; set; }
        public int BannerPositionX { get; set; }
        public int BannerPositionY { get; set; }
        public string ProfileThumbnailImagePath { get; set; }
        public string UserName { get; set; }
        public int AverageRatingValue { get; set; }
        public int ReviewCount { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get { return Birthday.GetAge(); } }
        public string Gender { get; set; }
        public string CityName { get; set; }
        public string StateAbbreviation { get; set; }
        public string CountryName { get; set; }
        public string Location { get { return CityExtensions.ToFullLocationString(CityName, StateAbbreviation, CountryName); } }
        public bool IsFavoritedByCurrentUser { get; set; }
        public int ProfileId { get; set; }

        #region Send message stuff

        public SendMessageViewModel SendMessage { get; set; }

        #endregion Send message stuff

        #region Image upload stuff

        public UploadImageViewModel UploadImage { get; set; }

        #endregion Image upload stuff
    }
}