using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace TwolipsDating.Utilities
{
    public class UploadedProfileImage : UploadedImage
    {
        public string ThumbnailFileName { get; private set; }

        public WebImage ThumbnailImage { get; private set; }

        public UploadedProfileImage(HttpPostedFileBase uploadedFile)
            : base(uploadedFile)
        {
            ThumbnailFileName = String.Format("{0}_{2}{1}", Guid, FileType, "thumb");

            ResizeImageIfNecessary(uploadedFile, 1000, 1000);
        }

        private void CreateThumbnail(HttpPostedFileBase uploadedFile)
        {
            WebImage image = new WebImage(uploadedFile.InputStream);

            // transform to a square by cropping
            if (image.Width > image.Height)
            {
                int cropAmount = (image.Width - image.Height) / 2;
                image = image.Crop(0, cropAmount, 0, cropAmount);
            }
            else
            {
                int cropAmount = (image.Height - image.Width) / 2;
                image = image.Crop(cropAmount, 0, cropAmount, 0);
            }

            image.Resize(200, 200, true);
        }
    }
}