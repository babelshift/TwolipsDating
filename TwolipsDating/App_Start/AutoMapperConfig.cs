using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            string cdn = ConfigurationManager.AppSettings["cdnUrl"];

            Mapper.CreateMap<EmailNotifications, ManageNotificationsViewModel>();

            Mapper.CreateMap<TwolipsDating.Models.Profile, ProfileViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => source.Birthday.GetAge()))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(source => source.Gender.Name))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => source.GeoCity.ToFullLocationString()))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.ProfileUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.GetProfileImagePath()))
                .ForMember(dest => dest.ProfileThumbnailImagePath, opts => opts.MapFrom(source => source.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.SelectedTitle, opts => opts.MapFrom(source => source.SelectedTitle != null ? source.SelectedTitle.Name : String.Empty))
                .ForMember(dest => dest.SelfDescription, opts => opts.MapFrom(source => source.SelfDescription));

            Mapper.CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.ReviewId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.AuthorProfileId, opts => opts.MapFrom(source => source.AuthorUser.Profile.Id))
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.RatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.AuthorUser.Profile.GetProfileThumbnailImagePath()));

            Mapper.CreateMap<UserImage, UserImageViewModel>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.FileName, opts => opts.MapFrom(source => source.FileName))
                .ForMember(dest => dest.Path, opts => opts.MapFrom(source => source.GetPath()))
                .ForMember(dest => dest.ThumbnailPath, opts => opts.MapFrom(source => source.GetThumbnailPath()))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()));

            Mapper.CreateMap<UserImage, UploadedImageFeedViewModel>()
                .ForMember(dest => dest.UploaderProfileImagePath, opts => opts.MapFrom(source => source.ApplicationUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.UploaderUserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateUploaded))
                .ForMember(dest => dest.UploaderProfileId, opts => opts.MapFrom(source => source.ApplicationUser.Profile.Id))
                .ForMember(dest => dest.UploaderUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .ForMember(dest => dest.UploadedImagesPaths, opts => opts.UseValue(new List<UploadedImageViewModel>()))
                .AfterMap((source, dest) => dest.UploadedImagesPaths.Add(new UploadedImageViewModel() { Path = source.GetPath(), ThumbnailPath = source.GetThumbnailPath() }));

            Mapper.CreateMap<Message, MessageFeedViewModel>()
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.ReceiverApplicationUser.UserName))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.MessageContent, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.Id))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.Id))
                .ForMember(dest => dest.SenderUserId, opts => opts.MapFrom(source => source.SenderApplicationUser.Id))
                .ForMember(dest => dest.ReceiverUserId, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Id));

            Mapper.CreateMap<Review, ReviewWrittenFeedViewModel>()
                .ForMember(dest => dest.ReviewId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.TargetUserName, opts => opts.MapFrom(source => source.TargetUser.UserName))
                .ForMember(dest => dest.TargetProfileImagePath, opts => opts.MapFrom(source => source.TargetUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.ReviewContent, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.ReviewRatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.AuthorProfileImagePath, opts => opts.MapFrom(source => source.AuthorUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateCreated))
                .ForMember(dest => dest.AuthorProfileId, opts => opts.MapFrom(source => source.AuthorUser.Profile.Id))
                .ForMember(dest => dest.TargetProfileId, opts => opts.MapFrom(source => source.TargetUser.Profile.Id));

            Mapper.CreateMap<GiftTransactionLog, GiftReceivedFeedViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateTransactionOccurred))
                .ForMember(dest => dest.Gifts, opts => opts.UseValue(new Dictionary<int, GiftReceivedFeedItemViewModel>()))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.ToUser.UserName))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ToUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ToUser.Profile.Id))
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.FromUser.UserName))
                .ForMember(dest => dest.SenderUserId, opts => opts.MapFrom(source => source.FromUser.Id))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.FromUser.Profile.Id))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.FromUser.Profile.GetProfileThumbnailImagePath()));

            Mapper.CreateMap<CompletedQuiz, CompletedQuizFeedViewModel>()
                .ForMember(dest => dest.DateCompleted, opts => opts.MapFrom(source => source.DateCompleted))
                .ForMember(dest => dest.QuizId, opts => opts.MapFrom(source => source.QuizId))
                .ForMember(dest => dest.QuizName, opts => opts.MapFrom(source => source.Quiz.Name))
                .ForMember(dest => dest.SourceProfileId, opts => opts.MapFrom(source => source.User.Profile.Id))
                .ForMember(dest => dest.SourceProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.SourceUserName, opts => opts.MapFrom(source => source.User.UserName));

            Mapper.CreateMap<TagSuggestion, TagSuggestionReceivedFeedViewModel>()
                .ForMember(dest => dest.DateSuggested, opts => opts.MapFrom(source => source.DateSuggested))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ProfileId))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.Profile.ApplicationUser.UserName))
                .ForMember(dest => dest.SuggestProfileId, opts => opts.MapFrom(source => source.SuggestingUser.Profile.Id))
                .ForMember(dest => dest.SuggestProfileImagePath, opts => opts.MapFrom(source => source.SuggestingUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.SuggestUserId, opts => opts.MapFrom(source => source.SuggestingUserId))
                .ForMember(dest => dest.SuggestUserName, opts => opts.MapFrom(source => source.SuggestingUser.UserName))
                .ForMember(dest => dest.Tags, opts => opts.UseValue(new List<string>()))
                .AfterMap((source, dest) => dest.Tags.Add(source.Tag.Name));

            Mapper.CreateMap<MilestoneAchievement, AchievementFeedViewModel>()
                .ForMember(dest => dest.DateAchieved, opts => opts.MapFrom(source => source.DateAchieved))
                .ForMember(dest => dest.AchievementName, opts => opts.MapFrom(source => String.Format("{0} ({1})", source.Milestone.MilestoneType.Name, source.Milestone.AmountRequired)))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.User.Profile.Id))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.UserProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetProfileThumbnailImagePath()));

            Mapper.CreateMap<Tag, TagViewModel>();

            Mapper.CreateMap<Tag, ProfileTagSuggestionViewModel>()
                .ForMember(dest => dest.TagName, opts => opts.MapFrom(source => source.Name))
                .ForMember(dest => dest.TagDescription, opts => opts.MapFrom(source => source.Description));

            Mapper.CreateMap<Message, ConversationItemViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.TargetProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.MostRecentMessageBody, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.MostRecentMessageStatusId, opts => opts.MapFrom(source => source.MessageStatusId))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()));

            Mapper.CreateMap<MessageConversation, ConversationItemViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.MostRecentMessageSenderUserId, opts => opts.MapFrom(source => source.SenderApplicationUserId))
                .ForMember(dest => dest.MostRecentMessageBody, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.MostRecentMessageStatusId, opts => opts.MapFrom(source => source.MessageStatusId))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()));

            Mapper.CreateMap<Message, ReceivedMessageViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Body, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.SenderName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.Id));

            Mapper.CreateMap<Message, SentMessageViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Body, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.ReceiverName, opts => opts.MapFrom(source => source.ReceiverApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.Id));

            Mapper.CreateMap<InventoryItem, InventoryItemViewModel>()
                .ForMember(dest => dest.GiftDescription, opts => opts.MapFrom(source => source.StoreItem.Description))
                .ForMember(dest => dest.GiftIconFilePath, opts => opts.MapFrom(source => source.StoreItem.GetIconPath()))
                .ForMember(dest => dest.GiftId, opts => opts.MapFrom(source => source.StoreItemId))
                .ForMember(dest => dest.GiftName, opts => opts.MapFrom(source => source.StoreItem.Name))
                .ForMember(dest => dest.InventoryItemId, opts => opts.MapFrom(source => source.InventoryItemId))
                .ForMember(dest => dest.ItemCount, opts => opts.MapFrom(source => source.ItemCount));

            Mapper.CreateMap<Question, QuestionViewModel>()
                .ForMember(dest => dest.QuestionId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.CorrectAnswerId, opts => opts.MapFrom(source => source.CorrectAnswerId))
                .ForMember(dest => dest.Points, opts => opts.MapFrom(source => source.Points))
                .ForMember(dest => dest.Answers, opts => opts.MapFrom(source => source.PossibleAnswers.ToList().AsReadOnly()));

            Mapper.CreateMap<Answer, AnswerViewModel>()
                .ForMember(dest => dest.AnswerId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content));

            Mapper.CreateMap<Quiz, QuizOverviewViewModel>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(source => source.Name))
                .ForMember(dest => dest.AveragePoints, opts => opts.MapFrom(source =>
                    source.Questions != null && source.Questions.Count > 0 ? (int)Math.Round(source.Questions.Average(x => x.Points)) : 0));

            Mapper.CreateMap<StoreItem, StoreItemViewModel>()
                .ForMember(dest => dest.ItemId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemName, opts => opts.MapFrom(source => source.Name))
                .ForMember(dest => dest.ItemImagePath, opts => opts.MapFrom(source => source.GetIconPath()))
                .ForMember(dest => dest.PointsCost, opts => opts.MapFrom(source => source.PointPrice))
                .ForMember(dest => dest.ItemDescription, opts => opts.MapFrom(source => source.Description))
                .ForMember(dest => dest.ItemTypeId, opts => opts.MapFrom(source => source.ItemTypeId))
                .ForMember(dest => dest.Discount, opts => opts.MapFrom(source => source.GetDiscountIfAvailable()))
                .ForMember(dest => dest.DateSaleEnds, opts => opts.MapFrom(source => source.GetDateSaleEndsIfAvailable()));

            Mapper.CreateMap<StoreTransactionLog, StoreTransactionViewModel>()
                .ForMember(dest => dest.TransactionDate, opts => opts.MapFrom(source => source.DateTransactionOccurred))
                .ForMember(dest => dest.ItemName, opts => opts.MapFrom(source => source.StoreItem.Name))
                .ForMember(dest => dest.ItemCost, opts => opts.MapFrom(source => source.StoreItem.PointPrice))
                .ForMember(dest => dest.ItemCount, opts => opts.MapFrom(source => source.ItemCount))
                .ForMember(dest => dest.ItemType, opts => opts.MapFrom(source => StoreItemTypeValues.Gift))
                .ForMember(dest => dest.TotalCost, opts => opts.MapFrom(source => source.ItemCount * source.StoreItem.PointPrice));

            Mapper.CreateMap<GiftTransactionLog, GiftTransactionViewModel>()
                .ForMember(dest => dest.GiftAmount, opts => opts.MapFrom(source => source.ItemCount))
                .ForMember(dest => dest.GiftImagePath, opts => opts.MapFrom(source => source.StoreItem.GetIconPath()))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.FromUser.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.FromUser.UserName))
                .ForMember(dest => dest.GiftTransactionId, opts => opts.MapFrom(source => source.GiftTransactionLogId))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.FromUser.Profile.Id))
                .ForMember(dest => dest.DateTransaction, opts => opts.MapFrom(source => source.DateTransactionOccurred.GetTimeAgo()));

            Mapper.CreateMap<AnsweredQuestion, UserAnsweredQuestionCorrectlyViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateAnswered.GetTimeAgo()))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.User.Profile.Id));

            Mapper.CreateMap<CompletedQuiz, UserCompletedQuizViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.QuizName, opts => opts.MapFrom(source => source.Quiz.Name))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCompleted.GetTimeAgo()))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetProfileThumbnailImagePath()))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.User.Profile.Id));

            Mapper.CreateMap<ShoppingCart, ShoppingCartViewModel>()
                .ForMember(dest => dest.Items, opts => opts.MapFrom(source => source.Items.ToList().AsReadOnly()));

            Mapper.CreateMap<ShoppingCartItem, ShoppingCartItemViewModel>()
                .ForMember(dest => dest.Item, opts => opts.MapFrom(source => source.Item));

            Mapper.CreateMap<StoreSale, StoreItemViewModel>()
                .ForMember(dest => dest.ItemId, opts => opts.MapFrom(source => source.StoreItem.Id))
                .ForMember(dest => dest.ItemName, opts => opts.MapFrom(source => source.StoreItem.Name))
                .ForMember(dest => dest.ItemImagePath, opts => opts.MapFrom(source => source.StoreItem.GetIconPath()))
                .ForMember(dest => dest.PointsCost, opts => opts.MapFrom(source => source.StoreItem.PointPrice))
                .ForMember(dest => dest.ItemDescription, opts => opts.MapFrom(source => source.StoreItem.Description))
                .ForMember(dest => dest.ItemTypeId, opts => opts.MapFrom(source => source.StoreItem.ItemTypeId))
                .ForMember(dest => dest.DateSaleEnds, opts => opts.MapFrom(source => source.DateEnd))
                .ForMember(dest => dest.Discount, opts => opts.MapFrom(source => source.Discount));
        }
    }
}