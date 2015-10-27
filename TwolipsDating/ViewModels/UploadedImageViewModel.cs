using TwolipsDating.Utilities;
namespace TwolipsDating.ViewModels
{
    public class UploadedImageViewModel
    {
        public string Path { get; set; }
        public string ThumbnailPath { get { return UserImageExtensions.GetThumbnailPath(Path); } }
    }
}