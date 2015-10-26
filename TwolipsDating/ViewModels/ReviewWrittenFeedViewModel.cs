using System;

namespace TwolipsDating.ViewModels
{
    public class ReviewWrittenFeedViewModel
    {
        public int ReviewId { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorSEOName { get; set; }
        public string AuthorProfileId { get; set; }
        public string AuthorProfileImagePath { get; set; }
        public string TargetUserName { get; set; }
        public string TargetSEOName { get; set; }
        public string TargetProfileId { get; set; }
        public string TargetProfileImagePath { get; set; }
        public int? ReviewRatingValue { get; set; }
        public string ReviewContent { get; set; }
        public string TimeAgo { get; set; }
        public DateTime DateOccurred { get; set; }
    }
}