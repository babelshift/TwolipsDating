using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public enum DashboardFeedItemType
    {
        Message,
        UploadedPictures,
        ReviewWritten
    }

    public class DashboardViewModel
    {
        public DateTime DateOccurred { get; set; }
        public DashboardFeedItemType ItemType { get; set; }
        public MessageFeedViewModel MessageFeedItem { get; set; }
        public UploadedImageFeedViewModel UploadedImageFeedItem { get; set; }
        public ReviewWrittenFeedViewModel ReviewWrittenFeedItem { get; set; }
    }
}