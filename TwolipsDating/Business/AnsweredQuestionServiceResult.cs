using System.Collections.Generic;

namespace TwolipsDating.Business
{
    public class AnsweredQuestionServiceResult : ServiceResult
    {
        public int CorrectAnswerId { get; private set; }

        public int TagsAwardedCount { get; set; }

        private AnsweredQuestionServiceResult(int correctAnswerId, int tagsAwardedCount)
        {
            CorrectAnswerId = correctAnswerId;
            TagsAwardedCount = tagsAwardedCount;
        }

        private AnsweredQuestionServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new AnsweredQuestionServiceResult Success(int correctAnswerId, int tagsAwardedCount)
        {
            return new AnsweredQuestionServiceResult(correctAnswerId, tagsAwardedCount);
        }

        public static new AnsweredQuestionServiceResult Failed(params string[] errors)
        {
            return new AnsweredQuestionServiceResult(errors);
        }
    }
}