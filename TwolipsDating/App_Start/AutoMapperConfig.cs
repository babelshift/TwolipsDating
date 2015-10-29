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

            Mapper.CreateMap<TwolipsDating.Models.Profile, FollowerViewModel>()
                .ForMember(dest => dest.BannerImagePath, opts => opts.MapFrom(source => source.BannerImage.GetPath()))
                .ForMember(dest => dest.BannerPositionX, opts => opts.MapFrom(source => source.BannerPositionX))
                .ForMember(dest => dest.BannerPositionY, opts => opts.MapFrom(source => source.BannerPositionY))
                .ForMember(dest => dest.ProfileThumbnailImagePath, opts => opts.MapFrom(source => source.GetThumbnailImagePath()))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.UserSummaryOfSelf, opts => opts.MapFrom(source => source.SummaryOfSelf));

            Mapper.CreateMap<TwolipsDating.Models.Profile, UserToMessageViewModel>()
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => source.GeoCity.ToFullLocationString()))
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => source.Birthday.GetAge()))
                .ForMember(dest => dest.ProfileThumbnailImagePath, opts => opts.MapFrom(source => source.GetThumbnailImagePath()))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(source => source.Gender.Name));

            Mapper.CreateMap<TwolipsDating.Models.Profile, ProfileViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => source.Birthday.GetAge()))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(source => source.Gender.Name))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => source.GeoCity.ToFullLocationString()))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.ProfileUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.GetImagePath()))
                .ForMember(dest => dest.ProfileThumbnailImagePath, opts => opts.MapFrom(source => source.GetThumbnailImagePath()))
                .ForMember(dest => dest.SelectedTitle, opts => opts.MapFrom(source => source.SelectedTitle != null ? source.SelectedTitle.Name : String.Empty))
                .ForMember(dest => dest.SelectedTitleImage, opts => opts.MapFrom(source => source.SelectedTitle != null ? source.SelectedTitle.GetImagePath() : String.Empty))
                .ForMember(dest => dest.SummaryOfSelf, opts => opts.MapFrom(source => source.SummaryOfSelf))
                .ForMember(dest => dest.SummaryOfDoing, opts => opts.MapFrom(source => source.SummaryOfDoing))
                .ForMember(dest => dest.SummaryOfGoing, opts => opts.MapFrom(source => source.SummaryOfGoing))
                .ForMember(dest => dest.LookingForType, opts => opts.MapFrom(source => source.LookingForType.Name))
                .ForMember(dest => dest.LookingForLocation, opts => opts.MapFrom(source => source.LookingForLocation.Range))
                .ForMember(dest => dest.RelationshipStatus, opts => opts.MapFrom(source => source.RelationshipStatus.Name))
                .ForMember(dest => dest.LookingForAgeMin, opts => opts.MapFrom(source => source.LookingForAgeMin))
                .ForMember(dest => dest.LookingForAgeMax, opts => opts.MapFrom(source => source.LookingForAgeMax))
                .ForMember(dest => dest.LastLoginTimeAgo, opts => opts.MapFrom(source => source.ApplicationUser.DateLastLogin.GetTimeAgo()))
                .ForMember(dest => dest.BannerImagePath, opts => opts.MapFrom(source => source.BannerImage.GetPath()))
                .ForMember(dest => dest.BannerPositionX, opts => opts.MapFrom(source => source.BannerPositionX))
                .ForMember(dest => dest.BannerPositionY, opts => opts.MapFrom(source => source.BannerPositionY))
                .ForMember(dest => dest.CurrentPoints, opts => opts.MapFrom(source => source.ApplicationUser.CurrentPoints))
                .ForMember(dest => dest.LifeTimePoints, opts => opts.MapFrom(source => source.ApplicationUser.LifetimePoints))
                .ForMember(dest => dest.Languages, opts => opts.MapFrom(source => source.Languages.Select(a => a.Name).ToList()));

            Mapper.CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.ReviewId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.AuthorProfileId, opts => opts.MapFrom(source => source.AuthorUser.Profile.Id))
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.RatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.AuthorUser.Profile.GetThumbnailImagePath()));

            Mapper.CreateMap<UserImage, UserImageViewModel>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.FileName, opts => opts.MapFrom(source => source.FileName))
                .ForMember(dest => dest.Path, opts => opts.MapFrom(source => source.GetPath()))
                .ForMember(dest => dest.ThumbnailPath, opts => opts.MapFrom(source => source.GetThumbnailPath()))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()));

            Mapper.CreateMap<UserImage, UploadedImageFeedViewModel>()
                .ForMember(dest => dest.UploaderProfileImagePath, opts => opts.MapFrom(source => source.ApplicationUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.UploaderUserName, opts => opts.MapFrom(source => source.ApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateUploaded.GetTimeAgo()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateUploaded))
                .ForMember(dest => dest.UploaderProfileId, opts => opts.MapFrom(source => source.ApplicationUser.Profile.Id))
                .ForMember(dest => dest.UploaderUserId, opts => opts.MapFrom(source => source.ApplicationUser.Id))
                .ForMember(dest => dest.UploadedImagesPaths, opts => opts.UseValue(new List<UploadedImageViewModel>()))
                .AfterMap((source, dest) => dest.UploadedImagesPaths.Add(new UploadedImageViewModel() { Path = source.GetPath() }));

            Mapper.CreateMap<Message, MessageFeedViewModel>()
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.ReceiverApplicationUser.UserName))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.GetThumbnailImagePath()))
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
                .ForMember(dest => dest.TargetProfileImagePath, opts => opts.MapFrom(source => source.TargetUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.AuthorUserName, opts => opts.MapFrom(source => source.AuthorUser.UserName))
                .ForMember(dest => dest.ReviewContent, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.ReviewRatingValue, opts => opts.MapFrom(source => source.RatingValue))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCreated.GetTimeAgo()))
                .ForMember(dest => dest.AuthorProfileImagePath, opts => opts.MapFrom(source => source.AuthorUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.DateOccurred, opts => opts.MapFrom(source => source.DateCreated))
                .ForMember(dest => dest.AuthorProfileId, opts => opts.MapFrom(source => source.AuthorUser.Profile.Id))
                .ForMember(dest => dest.TargetProfileId, opts => opts.MapFrom(source => source.TargetUser.Profile.Id));

            Mapper.CreateMap<GiftTransactionLog, GiftReceivedFeedViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateTransactionOccurred))
                .ForMember(dest => dest.Gifts, opts => opts.UseValue(new Dictionary<int, GiftReceivedFeedItemViewModel>()))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.ToUser.UserName))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ToUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ToUser.Profile.Id))
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.FromUser.UserName))
                .ForMember(dest => dest.SenderUserId, opts => opts.MapFrom(source => source.FromUser.Id))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.FromUser.Profile.Id))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.FromUser.Profile.Id))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.FromUser.Profile.GetThumbnailImagePath()));

            Mapper.CreateMap<CompletedQuiz, CompletedQuizFeedViewModel>()
                .ForMember(dest => dest.DateCompleted, opts => opts.MapFrom(source => source.DateCompleted))
                .ForMember(dest => dest.QuizId, opts => opts.MapFrom(source => source.QuizId))
                .ForMember(dest => dest.QuizName, opts => opts.MapFrom(source => source.Quiz.Name))
                .ForMember(dest => dest.SourceProfileId, opts => opts.MapFrom(source => source.User.Profile.Id))
                .ForMember(dest => dest.SourceProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.QuizThumbnailImagePath, opts => opts.MapFrom(source => source.Quiz.GetThumbnailImagePath()))
                .ForMember(dest => dest.SourceUserName, opts => opts.MapFrom(source => source.User.UserName));

            Mapper.CreateMap<TagSuggestion, TagSuggestionReceivedFeedViewModel>()
                .ForMember(dest => dest.DateSuggested, opts => opts.MapFrom(source => source.DateSuggested))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ProfileId))
                .ForMember(dest => dest.ReceiverUserName, opts => opts.MapFrom(source => source.Profile.ApplicationUser.UserName))
                .ForMember(dest => dest.SuggestProfileId, opts => opts.MapFrom(source => source.SuggestingUser.Profile.Id))
                .ForMember(dest => dest.SuggestProfileImagePath, opts => opts.MapFrom(source => source.SuggestingUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.SuggestUserId, opts => opts.MapFrom(source => source.SuggestingUserId))
                .ForMember(dest => dest.SuggestUserName, opts => opts.MapFrom(source => source.SuggestingUser.UserName))
                .ForMember(dest => dest.Tags, opts => opts.UseValue(new List<string>()))
                .AfterMap((source, dest) => dest.Tags.Add(source.Tag.Name));

            Mapper.CreateMap<MilestoneAchievement, AchievementFeedViewModel>()
                .ForMember(dest => dest.DateAchieved, opts => opts.MapFrom(source => source.DateAchieved))
                .ForMember(dest => dest.AchievementName, opts => opts.MapFrom(source => 
                    source.Milestone.AmountRequired > 1 
                    ? String.Format("{0} ({1})", source.Milestone.MilestoneType.Name, source.Milestone.AmountRequired)
                    : source.Milestone.MilestoneType.Name))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.User.Profile.Id))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.AchievementIconPath, opts => opts.MapFrom(source => source.Milestone.GetImagePath()))
                .ForMember(dest => dest.UserProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetThumbnailImagePath()));

            Mapper.CreateMap<FavoriteProfile, FollowerFeedViewModel>()
                .ForMember(dest => dest.DateFollowed, opts => opts.MapFrom(source => source.DateFavorited))
                .ForMember(dest => dest.FollowerProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.FollowerName, opts => opts.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.FollowerProfileId, opts => opts.MapFrom(source => source.User.Profile.Id));

            Mapper.CreateMap<Tag, TagViewModel>();

            Mapper.CreateMap<Tag, ProfileTagSuggestionViewModel>()
                .ForMember(dest => dest.TagName, opts => opts.MapFrom(source => source.Name))
                .ForMember(dest => dest.TagDescription, opts => opts.MapFrom(source => source.Description));

            Mapper.CreateMap<Message, ConversationItemViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.TargetProfileId, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.Id))
                .ForMember(dest => dest.TargetName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.TargetProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.MostRecentMessageBody, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.MostRecentMessageStatusId, opts => opts.MapFrom(source => source.MessageStatusId));

            Mapper.CreateMap<MessageConversation, ConversationItemViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.MostRecentMessageSenderUserId, opts => opts.MapFrom(source => source.SenderApplicationUserId))
                .ForMember(dest => dest.MostRecentMessageBody, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.MostRecentMessageStatusId, opts => opts.MapFrom(source => source.MessageStatusId));

            Mapper.CreateMap<Message, ReceivedMessageViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Body, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.SenderName, opts => opts.MapFrom(source => source.SenderApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.SenderApplicationUser.Profile.Id));

            Mapper.CreateMap<Message, SentMessageViewModel>()
                .ForMember(dest => dest.DateSent, opts => opts.MapFrom(source => source.DateSent))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Body, opts => opts.MapFrom(source => source.Body))
                .ForMember(dest => dest.ReceiverName, opts => opts.MapFrom(source => source.ReceiverApplicationUser.UserName))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateSent.GetTimeAgo()))
                .ForMember(dest => dest.ReceiverProfileImagePath, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.ReceiverProfileId, opts => opts.MapFrom(source => source.ReceiverApplicationUser.Profile.Id));

            Mapper.CreateMap<InventoryItem, InventoryItemViewModel>()
                .ForMember(dest => dest.GiftDescription, opts => opts.MapFrom(source => source.StoreItem.Description))
                .ForMember(dest => dest.GiftIconFilePath, opts => opts.MapFrom(source => source.StoreItem.GetImagePath()))
                .ForMember(dest => dest.GiftId, opts => opts.MapFrom(source => source.StoreItemId))
                .ForMember(dest => dest.GiftName, opts => opts.MapFrom(source => source.StoreItem.Name))
                .ForMember(dest => dest.InventoryItemId, opts => opts.MapFrom(source => source.InventoryItemId))
                .ForMember(dest => dest.ItemCount, opts => opts.MapFrom(source => source.ItemCount));

            Mapper.CreateMap<MinefieldQuestion, MinefieldQuestionViewModel>()
               .ForMember(dest => dest.MinefieldQuestionId, opts => opts.MapFrom(source => source.MinefieldQuestionId))
               .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
               .ForMember(dest => dest.Points, opts => opts.MapFrom(source => source.Points))
               .ForMember(dest => dest.Answers, opts => opts.MapFrom(source => 
                   source.PossibleAnswers
                   .OrderBy(x => Guid.NewGuid())
                   .ToList()
                   .AsReadOnly()));

            Mapper.CreateMap<MinefieldAnswer, MinefieldAnswerViewModel>()
                .ForMember(dest => dest.AnswerId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.Content, opts => opts.MapFrom(source => source.Content))
                .ForMember(dest => dest.IsCorrect, opts => opts.MapFrom(source => source.IsCorrect));

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
                .ForMember(dest => dest.QuizCategoryName, opts => opts.MapFrom(source => source.QuizCategory.Name))
                .ForMember(dest => dest.QuizCategoryId, opts => opts.MapFrom(source => source.QuizCategoryId))
                .ForMember(dest => dest.ThumbnailImagePath, opts => opts.MapFrom(source => source.GetThumbnailImagePath()))
                .ForMember(dest => dest.AveragePoints, opts => opts.ResolveUsing(source =>
                    {
                        if (source.QuizTypeId == (int)QuizTypeValues.Individual)
                        {
                            return source.Questions != null && source.Questions.Count > 0 ? (int)Math.Round(source.Questions.Average(x => x.Points)) : 0;
                        }
                        else
                        {
                            return source.MinefieldQuestion != null ? source.MinefieldQuestion.Points : 0;
                        }
                    }));

            Mapper.CreateMap<StoreItem, StoreItemViewModel>()
                .ForMember(dest => dest.ItemId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemName, opts => opts.MapFrom(source => source.Name))
                .ForMember(dest => dest.ItemImagePath, opts => opts.MapFrom(source => source.GetImagePath()))
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
                .ForMember(dest => dest.GiftImagePath, opts => opts.MapFrom(source => source.StoreItem.GetImagePath()))
                .ForMember(dest => dest.SenderProfileImagePath, opts => opts.MapFrom(source => source.FromUser.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.SenderUserName, opts => opts.MapFrom(source => source.FromUser.UserName))
                .ForMember(dest => dest.GiftTransactionId, opts => opts.MapFrom(source => source.GiftTransactionLogId))
                .ForMember(dest => dest.SenderProfileId, opts => opts.MapFrom(source => source.FromUser.Profile.Id))
                .ForMember(dest => dest.DateTransaction, opts => opts.MapFrom(source => source.DateTransactionOccurred.GetTimeAgo()));

            Mapper.CreateMap<AnsweredQuestion, UserAnsweredQuestionCorrectlyViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.User.Profile.Id))
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(source => source.User.Id))
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => source.User.Profile.Birthday.GetAge()))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => source.User.Profile.GeoCity.ToFullLocationString()));

            Mapper.CreateMap<CompletedQuiz, UserCompletedQuizViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.QuizName, opts => opts.MapFrom(source => source.Quiz.Name))
                .ForMember(dest => dest.TimeAgo, opts => opts.MapFrom(source => source.DateCompleted.GetTimeAgo()))
                .ForMember(dest => dest.ProfileImagePath, opts => opts.MapFrom(source => source.User.Profile.GetThumbnailImagePath()))
                .ForMember(dest => dest.ProfileId, opts => opts.MapFrom(source => source.User.Profile.Id))
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(source => source.User.Id))
                .ForMember(dest => dest.Age, opts => opts.MapFrom(source => source.User.Profile.Birthday.GetAge()))
                .ForMember(dest => dest.Location, opts => opts.MapFrom(source => source.User.Profile.GeoCity.ToFullLocationString()));

            Mapper.CreateMap<ShoppingCart, ShoppingCartViewModel>()
                .ForMember(dest => dest.Items, opts => opts.MapFrom(source => source.Items.ToList().AsReadOnly()));

            Mapper.CreateMap<ShoppingCartItem, ShoppingCartItemViewModel>()
                .ForMember(dest => dest.Item, opts => opts.MapFrom(source => source.Item));

            Mapper.CreateMap<StoreSale, StoreItemViewModel>()
                .ForMember(dest => dest.ItemId, opts => opts.MapFrom(source => source.StoreItem.Id))
                .ForMember(dest => dest.ItemName, opts => opts.MapFrom(source => source.StoreItem.Name))
                .ForMember(dest => dest.ItemImagePath, opts => opts.MapFrom(source => source.StoreItem.GetImagePath()))
                .ForMember(dest => dest.PointsCost, opts => opts.MapFrom(source => source.StoreItem.PointPrice))
                .ForMember(dest => dest.ItemDescription, opts => opts.MapFrom(source => source.StoreItem.Description))
                .ForMember(dest => dest.ItemTypeId, opts => opts.MapFrom(source => source.StoreItem.ItemTypeId))
                .ForMember(dest => dest.DateSaleEnds, opts => opts.MapFrom(source => source.DateEnd))
                .ForMember(dest => dest.Discount, opts => opts.MapFrom(source => source.Discount));

            Mapper.CreateMap<QuizCategory, QuizCategoryViewModel>()
                .ForMember(dest => dest.QuizCategoryId, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.QuizIcon, opts => opts.MapFrom(source => source.FontAwesomeIconName))
                .ForMember(dest => dest.QuizName, opts => opts.MapFrom(source => source.Name));
        }
    }
}