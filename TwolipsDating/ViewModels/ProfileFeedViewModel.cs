using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ProfileFeedViewModel
    {
        public string OriginatorUserName { get; set; }
        public string OriginatorProfileImagePath { get; set; }
        public string TargetUserName { get; set; }
        public string TargetProfileImagePath { get; set; }
        public IReadOnlyCollection<string> UploadedImagesPaths { get; set; }
        public int? ReviewRatingValue { get; set; }
        public string ReviewContent { get; set; }
        public string TimeAgo { get; set; }
        public DateTime DateOccurred { get; set; }
    }
}