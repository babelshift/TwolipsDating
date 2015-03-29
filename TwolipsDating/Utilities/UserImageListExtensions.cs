using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Utilities
{
    public static class UserImageListExtensions
    {
        public static IReadOnlyCollection<UploadedImageFeedViewModel> GetConsolidatedImagesForFeed(this IReadOnlyCollection<UserImage> collection)
        {
            var uploadedImageFeedViewModel = Mapper.Map<IReadOnlyCollection<UserImage>, IReadOnlyCollection<UploadedImageFeedViewModel>>(collection);

            Dictionary<UploadedImageFeedKey, UploadedImageFeedViewModel> userImagesViewModelConsolidated = new Dictionary<UploadedImageFeedKey, UploadedImageFeedViewModel>();
            foreach (var uploadedImageFeed in uploadedImageFeedViewModel)
            {
                UploadedImageFeedKey uploadedImageFeedKey = new UploadedImageFeedKey()
                {
                    UserId = uploadedImageFeed.UploaderUserId,
                    TimeAgo = uploadedImageFeed.TimeAgo
                };

                UpdateUserImageCollection(userImagesViewModelConsolidated, uploadedImageFeed, uploadedImageFeedKey);
            }

            return userImagesViewModelConsolidated.Values.ToList().AsReadOnly();
        }

        private static void UpdateUserImageCollection(Dictionary<UploadedImageFeedKey, UploadedImageFeedViewModel> userImagesViewModelConsolidated, UploadedImageFeedViewModel uploadedImageFeed, UploadedImageFeedKey key)
        {
            UploadedImageFeedViewModel existingUploadedImageFeed = new UploadedImageFeedViewModel();
            bool alreadyInCollection = userImagesViewModelConsolidated.TryGetValue(key, out existingUploadedImageFeed);
            if (!alreadyInCollection)
            {
                userImagesViewModelConsolidated.Add(key, uploadedImageFeed);
            }
            else
            {
                if (uploadedImageFeed.UploadedImagesPaths.Count > 0)
                {
                    existingUploadedImageFeed.UploadedImagesPaths.Add(uploadedImageFeed.UploadedImagesPaths[0]);
                }
            }
        }
    }
}