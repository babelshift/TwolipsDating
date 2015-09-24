using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public enum MilestoneTypeValues
    {
        QuestionsAnsweredCorrectly = 1,
        QuizzesCompletedSuccessfully = 2,
        ProfileReviewsWritten = 3,
        ProfileImagesUploaded = 4,
        GiftSent = 5,
        GiftsPurchased = 6,
        TitlesPurchased = 7,
        TagsAwarded = 8,
        PointsObtained = 9,
        Trekkie = 10,
        RebelAlliance = 11
    }

    public class MilestoneType
    {
        public int Id { get; set; }

        [Index("UX_Name", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Milestone> Milestones { get; set; }
    }
}