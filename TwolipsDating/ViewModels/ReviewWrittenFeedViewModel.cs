using System;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class ReviewWrittenFeedViewModel
    {
        public int ReviewId { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorSEOName { get { return ProfileExtensions.GetProfileSEOName(AuthorUserName); } }
        public int AuthorProfileId { get; set; }
        public string AuthorProfileImagePath { get; set; }
        public string TargetUserName { get; set; }
        public string TargetSEOName { get { return ProfileExtensions.GetProfileSEOName(TargetUserName); } }
        public int TargetProfileId { get; set; }
        public string TargetProfileImagePath { get; set; }
        public int? ReviewRatingValue { get; set; }
        public string ReviewContent { get; set; }
        public string TimeAgo { get { return DateOccurred.GetTimeAgo(); } }
        public DateTime DateOccurred { get; set; }
    }
}