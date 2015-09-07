using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace TwolipsDating.Utilities
{
    public abstract class UploadedImage
    {
        public string FileType { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public WebImage FullImage { get; protected set; }

        protected Guid Guid { get; private set; }

        public bool IsValidImage
        {
            get
            {
                if (ContentType != "image/jpeg"
                    && ContentType != "image/png"
                    && ContentType != "image/bmp"
                    && ContentType != "image/gif")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public UploadedImage(HttpPostedFileBase uploadedFile)
        {
            if (uploadedFile == null)
            {
                throw new ArgumentNullException("uploadedFile");
            }

            FileType = Path.GetExtension(uploadedFile.FileName);

            if (String.IsNullOrEmpty(FileType))
            {
                throw new InvalidOperationException("Uploaded file has no file extension.");
            }

            Guid = Guid.NewGuid();
            FileName = String.Format("{0}{1}", Guid, FileType);
            ContentType = uploadedFile.ContentType;

            if(!IsValidImage)
            {
                throw new InvalidOperationException("Uploaded file is not a valid image of type jpeg, png, gif, or bmp.");
            }
        }

        protected void ResizeImageIfNecessary(HttpPostedFileBase uploadedFile, int maxWidth, int maxHeight)
        {
            WebImage image = new WebImage(uploadedFile.InputStream);

            if (image.Width > maxWidth || image.Height > maxHeight)
            {
                image = image.Resize(maxWidth, maxHeight, true);
            }

            FullImage = image;
        }
    }
}