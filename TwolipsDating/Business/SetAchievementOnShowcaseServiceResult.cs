using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class SetAchievementOnShowcaseServiceResult : ServiceResult
    {
        public AchievementShowcaseItemViewModel NewAchievementOnShowcase { get; private set; }

        private SetAchievementOnShowcaseServiceResult(AchievementShowcaseItemViewModel newAchievementOnShowcase)
        {
            NewAchievementOnShowcase = newAchievementOnShowcase;
        }

        private SetAchievementOnShowcaseServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new SetAchievementOnShowcaseServiceResult Success(AchievementShowcaseItemViewModel newAchievementOnShowcase)
        {
            return new SetAchievementOnShowcaseServiceResult(newAchievementOnShowcase);
        }

        public static new SetAchievementOnShowcaseServiceResult Failed(params string[] errors)
        {
            return new SetAchievementOnShowcaseServiceResult(errors);
        }
    }
}