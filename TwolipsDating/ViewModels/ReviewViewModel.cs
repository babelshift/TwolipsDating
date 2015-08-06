namespace TwolipsDating.ViewModels
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public int AuthorProfileId { get; set; }
        public string AuthorUserName { get; set; }
        public string TimeAgo { get; set; }
        public int RatingValue { get; set; }
        public string Content { get; set; }
        public string ProfileImagePath { get; set; }
    }
}