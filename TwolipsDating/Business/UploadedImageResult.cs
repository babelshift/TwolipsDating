using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class UploadedImageResult : ServiceResult
    {
        public int UploadedImageId { get; private set; }

        private UploadedImageResult(int correctAnswerId)
        {
            UploadedImageId = correctAnswerId;
        }

        private UploadedImageResult(IEnumerable<string> errors)
            : base(errors) { }

        public static UploadedImageResult Success(int uploadedImageId)
        {
            return new UploadedImageResult(uploadedImageId);
        }

        public static UploadedImageResult Failed(params string[] errors)
        {
            return new UploadedImageResult(errors);
        }
    }
}