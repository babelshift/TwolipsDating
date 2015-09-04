using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace TwolipsDating.Utilities
{
    public class UploadedBannerImage : UploadedImage
    {
        public UploadedBannerImage(HttpPostedFileBase uploadedFile)
            : base(uploadedFile)
        {
            ResizeImageIfNecessary(uploadedFile, 1170, 1170);
        }
    }
}