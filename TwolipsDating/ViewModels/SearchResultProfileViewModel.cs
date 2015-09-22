namespace TwolipsDating.ViewModels
{
    public class SearchResultProfileViewModel
    {
        public string BannerImagePath { get; set; }
        public int? BannerPositionX { get; set; }
        public int? BannerPositionY { get; set; }
        public string ProfileThumbnailImagePath { get; set; }
        public string UserName { get; set; }
        public string UserSummaryOfSelf { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; }
        public int ProfileId { get; set; }
        public string UserId { get; set; }
    }
}