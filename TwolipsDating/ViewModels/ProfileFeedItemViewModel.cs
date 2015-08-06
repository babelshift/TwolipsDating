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
    }
}