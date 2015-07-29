using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class QuestionViolationViewModel
    {
        [Required]
        [Display(Name = "What's the problem?")]
        public int ViolationTypeId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public IDictionary<int, string> ViolationTypes { get; set; }
    }
}