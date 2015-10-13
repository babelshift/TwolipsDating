using System.Collections.Generic;

namespace TwolipsDating.Business
{
    public class AnsweredQuestionServiceResult : ServiceResult
    {
        public int CorrectAnswerId { get; private set; }

        public int TagsAwardedCount { get; private set; }

        public IReadOnlyCollection<AwardAchievementServiceResult> AwardedAchievements { get; private set; }

        private AnsweredQuestionServiceResult(int correctAnswerId, int tagsAwardedCount, IReadOnlyCollection<AwardAchievementServiceResult> awardedAchievements)
        {
            CorrectAnswerId = correctAnswerId;
            TagsAwardedCount = tagsAwardedCount;
            AwardedAchievements = awardedAchievements;
        }

        private AnsweredQuestionServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new AnsweredQuestionServiceResult Success(int correctAnswerId, int tagsAwardedCount, IReadOnlyCollection<AwardAchievementServiceResult> awardedAchievements)
        {
            return new AnsweredQuestionServiceResult(correctAnswerId, tagsAwardedCount, awardedAchievements);
        }

        public static new AnsweredQuestionServiceResult Failed(params string[] errors)
        {
            return new AnsweredQuestionServiceResult(errors);
        }
    }
}