using System;
using System.Collections.Generic;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class UploadedImageFeedViewModel
    {
        public DateTime DateOccurred { get; set; }
        public string UploaderUserId { get; set; }
        public int UploaderProfileId { get; set; }
        public string UploaderUserName { get; set; }
        public string UploaderProfileImagePath { get; set; }
        public string UploaderSEOName { get { return ProfileExtensions.ToSEOName(UploaderUserName); } }
        public string TimeAgo { get { return DateOccurred.GetTimeAgo(); } }
        public string Path { get; set; }
        public string ThumbnailPath { get { return UserImageExtensions.GetThumbnailPath(Path); } }
        public IList<UploadedImageViewModel> UploadedImagesPaths { get; set; }
    }
}