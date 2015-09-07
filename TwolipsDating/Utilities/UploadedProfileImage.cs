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
            CreateThumbnail(uploadedFile);
        }

        private void CreateThumbnail(HttpPostedFileBase uploadedFile)
        {
            ThumbnailImage = new WebImage(FullImage.GetBytes());

            // transform to a square by cropping
            if (ThumbnailImage.Width > ThumbnailImage.Height)
            {
                int cropAmount = (ThumbnailImage.Width - ThumbnailImage.Height) / 2;
                ThumbnailImage = ThumbnailImage.Crop(0, cropAmount, 0, cropAmount);
            }
            else
            {
                int cropAmount = (ThumbnailImage.Height - ThumbnailImage.Width) / 2;
                ThumbnailImage = ThumbnailImage.Crop(cropAmount, 0, cropAmount, 0);
            }

            ThumbnailImage.Resize(200, 200, true);
        }
    }
}