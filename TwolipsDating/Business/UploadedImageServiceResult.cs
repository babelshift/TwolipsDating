using System.Collections.Generic;

namespace TwolipsDating.Business
{
    public class UploadedImageServiceResult : ServiceResult
    {
        public int UploadedImageId { get; private set; }

        private UploadedImageServiceResult(int correctAnswerId)
        {
            UploadedImageId = correctAnswerId;
        }

        private UploadedImageServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new UploadedImageServiceResult Success(int uploadedImageId)
        {
            return new UploadedImageServiceResult(uploadedImageId);
        }

        public static new UploadedImageServiceResult Failed(params string[] errors)
        {
            return new UploadedImageServiceResult(errors);
        }
    }
}