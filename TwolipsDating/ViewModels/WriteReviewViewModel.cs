using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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