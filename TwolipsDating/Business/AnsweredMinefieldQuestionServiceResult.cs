using System.Collections.Generic;

namespace TwolipsDating.Business
{
    public class AnsweredMinefieldQuestionServiceResult : ServiceResult
    {
        public int CorrectAnswerCount { get; private set; }

        private AnsweredMinefieldQuestionServiceResult(int correctAnswerId)
        {
            CorrectAnswerCount = correctAnswerId;
        }

        private AnsweredMinefieldQuestionServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new AnsweredMinefieldQuestionServiceResult Success(int correctAnswerCount)
        {
            return new AnsweredMinefieldQuestionServiceResult(correctAnswerCount);
        }

        public static new AnsweredMinefieldQuestionServiceResult Failed(params string[] errors)
        {
            return new AnsweredMinefieldQuestionServiceResult(errors);
        }
    }
}