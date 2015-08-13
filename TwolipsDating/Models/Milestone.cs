using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public enum MilestoneValues
    {
        QuestionsAnsweredCorrectly1 = 1,
        QuestionsAnsweredCorrectly2 = 2,
        QuestionsAnsweredCorrectly3 = 3,
        QuestionsAnsweredCorrectly4 = 4,
        QuizzesCompletedSuccessfully1 = 5,
        QuizzesCompletedSuccessfully2 = 6,
        QuizzesCompletedSuccessfully3 = 7,
        QuizzesCompletedSuccessfully4 = 8,
        ProfileReviewsWritten1 = 9,
        ProfileReviewsWritten2 = 10,
        ProfileReviewsWritten3 = 11,
        ProfileReviewsWritten4 = 12,
        ProfileImagesUploaded1 = 13,
        ProfileImagesUploaded2 = 14,
        ProfileImagesUploaded3 = 15,
        ProfileImagesUploaded4 = 16,
        GiftsSent1 = 17,
        GiftsSent2 = 18,
        GiftsSent3 = 19,
        GiftsSent4 = 20,
        GiftsPurchased1 = 21,
        GiftsPurchased2 = 22,
        GiftsPurchased3 = 23,
        GiftsPurchased4 = 24,
        TitlesPurchased1 = 25,
        TitlesPurchased2 = 26,
        TitlesPurchased3 = 27,
        TitlesPurchased4 = 28,
        TagsAwarded1 = 29,
        TagsAwarded2 = 30,
        TagsAwarded3 = 31,
        TagsAwarded4 = 32,
        PointsObtained1 = 33,
        PointsObtained2 = 34,
        PointsObtained3 = 35,
        PointsObtained4 = 36,
    }

    public class Milestone
    {
        public int Id { get; set; }

        [Index("UX_AmountAndType", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int AmountRequired { get; set; }

        [Index("UX_AmountAndType", 2, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int MilestoneTypeId { get; set; }

        public virtual MilestoneType MilestoneType { get; set; }

        public virtual ICollection<MilestoneAchievement> MilestonesAchieved { get; set; }
    }
}