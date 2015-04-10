using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class WriteReviewViolationViewModel
    {
        [Required]
        [Display(Name = "Content:")]
        public string ViolationContent { get; set; }

        [Required]
        [Display(Name = "Violation Type:")]
        public int ViolationTypeId { get; set; }

        [Required]
        public int ReviewId { get; set; }

        public IDictionary<int, string> ViolationTypes { get; set; }
    }
}