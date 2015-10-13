using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class AwardAchievementServiceResult : ServiceResult
    {
        public IReadOnlyCollection<int> MilestoneIdsUnlocked { get; private set;}

        private AwardAchievementServiceResult(IReadOnlyCollection<int> milestoneIdsUnlocked)
        {
            MilestoneIdsUnlocked = milestoneIdsUnlocked;
        }

        private AwardAchievementServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new AwardAchievementServiceResult Success(IReadOnlyCollection<int> milestoneIdsUnlocked)
        {
            return new AwardAchievementServiceResult(milestoneIdsUnlocked);
        }

        public static new AwardAchievementServiceResult Failed(params string[] errors)
        {
            return new AwardAchievementServiceResult(errors);
        }
    }
}