using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ChangeProfileImageViewModel
    {
        [Required]
        public int UserImageId { get; set; }

        [Required]
        public int ProfileId { get; set; }
    }
}