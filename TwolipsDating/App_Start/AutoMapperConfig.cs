using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
using TwolipsDating.Utilities;
using System.Configuration;

namespace TwolipsDating
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            string cdn = ConfigurationManager.AppSettings["cdnUrl"];

            Mapper.CreateMap<TwolipsDating.Models.Profile, ProfileViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => (int)((DateTime.Now - source.Birthday).TotalDays / 365)))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(source => source.Gender.Name))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => String.Format("{0}, {1}", source.City.Name, source.City.USState.Abbreviation)))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.ProfileUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.UserImage.FileName)));

            Mapper.CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.RatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.AuthorUser.Profile.UserImage.FileName)));

            Mapper.CreateMap<UserImage, UserImageViewModel>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Path, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.FileName)))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()));

            Mapper.CreateMap<UserImage, UploadedImageFeedViewModel>()
                .ForMember(dest => dest.OriginatorProfileImagePath, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.ApplicationUser.Profile.UserImage.FileName)))
                .ForMember(dest => dest.OriginatorUserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.UploadedImagesPaths, opts => opts.MapFrom(source => new List<string>() { String.Format("{0}/{1}", cdn, source.FileName) }))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateUploaded));

            Mapper.CreateMap<Message, MessageFeedViewModel>()
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.SenderApplicationUser.Profile.UserImage.FileName)))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.ReceiverApplicationUser.UserName))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.ReceiverApplicationUser.Profile.UserImage.FileName)))
                .ForMember(dest => dest.MessageContent, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateSent));

            Mapper.CreateMap<Review, ReviewWrittenFeedViewModel>()
                .ForMember(dest => dest.TargetUserName, opts => opts.MapFrom(source => source.TargetUser.UserName))
                .ForMember(dest => dest.TargetProfileImagePath, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.TargetUser.Profile.UserImage.FileName)))
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.ReviewContent, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.ReviewRatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.AuthorProfileImagePath, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.AuthorUser.Profile.UserImage.FileName)))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateCreated));
        }
    }
}