using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public enum MilestoneTypeValues
    {
        QuestionAnsweredCorrectly = 1,
        QuizCompletedSuccessfully = 2
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