using System.ComponentModel.DataAnnotations;

namespace TwolipsDating.ViewModels
{
    public class WriteReviewViewModel
    {
        [Required]
        [Display(Name = "Content:")]
        public string ReviewContent { get; set; }

        [Required]
        [Display(Name = "Rating:")]
        public int RatingValue { get; set; }
    }
}