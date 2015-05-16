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
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => source.Birthday.GetAge()))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(source => source.Gender.Name))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => source.City.GetCityAndState()))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.ProfileUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.GetProfileImagePath()));

            Mapper.CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.AuthorProfileId, opts => opts.MapFrom(source => source.AuthorUser.Profile.Id))
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.RatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.AuthorUser.Profile.GetProfileImagePath()));

            Mapper.CreateMap<UserImage, UserImageViewModel>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.FileName, opts => opts.MapFrom(source => source.FileName))
                .ForMember(dest => dest.Path, opts => opts.MapFrom(source => String.Format("{0}/{1}", cdn, source.FileName)))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()));

            Mapper.CreateMap<UserImage, UploadedImageFeedViewModel>()
                .ForMember(dest => dest.UploaderProfileImagePath, opts => opts.MapFrom(source => source.ApplicationUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.UploaderUserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateUploaded))
                .ForMember(dest => dest.UploaderProfileId, opts => opts.MapFrom(source => source.ApplicationUser.Profile.Id))
                .ForMember(dest => dest.UploaderUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .AfterMap((source, dest) => dest.UploadedImagesPaths = new List<string> { String.Format("{0}/{1}", cdn, source.FileName) });

            Mapper.CreateMap<Message, MessageFeedViewModel>()
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.ReceiverApplicationUser.UserName))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.MessageContent, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.Id))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.Id));

            Mapper.CreateMap<Review, ReviewWrittenFeedViewModel>()
                .ForMember(dest => dest.ReviewId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.TargetUserName, opts => opts.MapFrom(source => source.TargetUser.UserName))
                .ForMember(dest => dest.TargetProfileImagePath, opts => opts.MapFrom(source => source.TargetUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.ReviewContent, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.ReviewRatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.AuthorProfileImagePath, opts => opts.MapFrom(source => source.AuthorUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateCreated))
                .ForMember(dest => dest.AuthorProfileId, opts => opts.MapFrom(source => source.AuthorUser.Profile.Id))
                .ForMember(dest => dest.TargetProfileId, opts => opts.MapFrom(source => source.TargetUser.Profile.Id));

            Mapper.CreateMap<Tag, TagViewModel>();

            Mapper.CreateMap<Tag, ProfileTagSuggestionViewModel>()
                .ForMember(dest => dest.TagName, opts => opts.MapFrom(source => source.Name));

            Mapper.CreateMap<Message, ConversationItemViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.TargetProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.MostRecentMessageBody, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()));

            Mapper.CreateMap<MessageConversation, ConversationItemViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.MostRecentMessageBody, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()));

            Mapper.CreateMap<Message, ReceivedMessageViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Body, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.SenderName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.Id));
            
            Mapper.CreateMap<Message, SentMessageViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Body, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.ReceiverName, opts => opts.MapFrom(source => source.ReceiverApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.GetProfileImagePath()))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.Id));

            Mapper.CreateMap<InventoryItem, InventoryItemViewModel>()
                .ForMember(dest => dest.GiftDescription, opts => opts.MapFrom(source => source.Gift.Description))
                .ForMember(dest => dest.GiftIconFilePath, opts => opts.MapFrom(source => source.Gift.GetIconPath()))
                .ForMember(dest => dest.GiftId, opts => opts.MapFrom(source => source.GiftId))
                .ForMember(dest => dest.GiftName, opts => opts.MapFrom(source => source.Gift.Name))
                .ForMember(dest => dest.InventoryItemId, opts => opts.MapFrom(source => source.InventoryItemId))
                .ForMember(dest => dest.ItemCount, opts => opts.MapFrom(source => source.ItemCount));

            Mapper.CreateMap<Question, QuestionViewModel>()
                .ForMember(dest => dest.QuestionId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.Answers, opts => opts.MapFrom(source => source.PossibleAnswers.ToList().AsReadOnly()));

            Mapper.CreateMap<Answer, AnswerViewModel>()
                .ForMember(dest => dest.AnswerId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content));

            Mapper.CreateMap<Quiz, QuizOverviewViewModel>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(source => source.Name))
                .ForMember(dest => dest.Points, opts => opts.MapFrom(source => source.Points));

            Mapper.CreateMap<Question, QuizQuestionViewModel>()
                .ForMember(dest => dest.QuestionId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.Answers, opts => opts.MapFrom(source => source.PossibleAnswers.ToList().AsReadOnly()));
        }
    }
}