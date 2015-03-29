using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Utilities
{
    public class UploadedImageFeedKey
    {
        public string UserId { get; set; }
        public string TimeAgo { get; set; }

        public override bool Equals(object obj)
        {
            var uploadedImageFeed = obj as UploadedImageFeedKey;
            if (uploadedImageFeed != null)
            {
                if(UserId == uploadedImageFeed.UserId && TimeAgo == uploadedImageFeed.TimeAgo)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = UserId.GetHashCode() + TimeAgo.GetHashCode();
            return hash;
        }
    }
}