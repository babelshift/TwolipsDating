using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public enum ProfileFeedItemType
    {
        UploadedPictures,
        ReviewWritten
    }

    public class ProfileFeedViewModel
    {
        public DateTime DateOccurred { get; set; }
        public DashboardFeedItemType ItemType { get; set; }
        public UploadedImageFeedViewModel UploadedImageFeedItem { get; set; }
        public ReviewWrittenFeedViewModel ReviewWrittenFeedItem { get; set; }
    }
}