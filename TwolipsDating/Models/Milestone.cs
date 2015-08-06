using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class Milestone
    {
        public int Id { get; set; }

        [Index("UX_PointsAndType", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int PointsRequired { get; set; }

        [Index("UX_PointsAndType", 2, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int MilestoneTypeId { get; set; }

        public virtual MilestoneType MilestoneType { get; set; }

        public virtual ICollection<MilestoneAchievement> MilestonesAchieved { get; set; }
    }
}