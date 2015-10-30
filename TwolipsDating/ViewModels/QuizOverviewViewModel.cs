using TwolipsDating.Utilities;
namespace TwolipsDating.ViewModels
{
    public class QuizOverviewViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SEOName { get { return QuizExtensions.ToSEOName(Name); } }
        public string Description { get; set; }
        public int AveragePoints { get; set; }
        public bool IsComplete { get; set; }
        public string ThumbnailImagePath { get; set; }
        public int QuizCategoryId { get; set; }
        public string QuizCategoryName { get; set; }
        public string QuizCategorySEOName { get { return QuizExtensions.ToSEOName(QuizCategoryName); } }
    }
}