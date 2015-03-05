using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
using TwolipsDating.Utilities;

namespace TwolipsDating
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.CreateMap<TwolipsDating.Models.Profile, ProfileViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => (int)((DateTime.Now - source.Birthday).TotalDays / 365)))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(source => source.Gender.Name))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => String.Format("{0}, {1}", source.City.Name, source.City.USState.Abbreviation)))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.ProfileUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id));

            Mapper.CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.TargetUser.UserName))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.RatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()));
        }
    }
}