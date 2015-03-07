using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UploadImageViewModel
    {
        [Required]
        public HttpPostedFileBase UploadedImage { get; set; }

        public IReadOnlyCollection<UserImageViewModel> UserImages { get; set; }
    }
}