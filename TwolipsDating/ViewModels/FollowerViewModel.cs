using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class FollowerViewModel
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