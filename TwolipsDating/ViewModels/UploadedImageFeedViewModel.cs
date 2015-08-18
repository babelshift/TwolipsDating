using System;
using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class UploadedImageFeedViewModel
    {
        public DateTime DateOccurred { get; set; }
        public string UploaderUserId { get; set; }
        public int UploaderProfileId { get; set; }
        public string UploaderUserName { get; set; }
        public string UploaderProfileImagePath { get; set; }
        public string TimeAgo { get; set; }
        public IList<UploadedImageViewModel> UploadedImagesPaths { get; set; }
    }
}