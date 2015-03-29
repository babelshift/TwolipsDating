﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UploadImageViewModel
    {
        public string ProfileUserId { get; set; }
        public string CurrentUserId { get; set; }

        [Required(ErrorMessage="You have to select an image to upload.")]
        public HttpPostedFileBase UploadedImage { get; set; }

        public IReadOnlyCollection<UserImageViewModel> UserImages { get; set; }
    }
}