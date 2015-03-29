﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwolipsDating.ViewModels
{
    public class UploadedImageFeedViewModel
    {
        public DateTime DateOccurred { get; set; }
        public int UploaderProfileId { get; set; }
        public string UploaderUserName { get; set; }
        public string UploaderProfileImagePath { get; set; }
        public string TimeAgo { get; set; }
        public IReadOnlyCollection<string> UploadedImagesPaths { get; set; }
    }
}