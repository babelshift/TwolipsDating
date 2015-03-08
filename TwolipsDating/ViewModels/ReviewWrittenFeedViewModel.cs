using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwolipsDating.ViewModels
{
    public class ReviewWrittenFeedViewModel
    {
        public string AuthorUserName { get; set; }
        public string AuthorProfileImagePath { get; set; }
        public string TargetUserName { get; set; }
        public string TargetProfileImagePath { get; set; }
        public int? ReviewRatingValue { get; set; }
        public string ReviewContent { get; set; }
        public string TimeAgo { get; set; }
        public DateTime DateOccurred { get; set; }
    }
}
