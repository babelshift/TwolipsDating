using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class AnsweredQuestionServiceResult : ServiceResult
    {
        public int CorrectAnswerId { get; private set; }

        private AnsweredQuestionServiceResult(int correctAnswerId)
        {
            CorrectAnswerId = correctAnswerId;
        }

        private AnsweredQuestionServiceResult(IEnumerable<string> errors)
            : base(errors) { }

        public static new AnsweredQuestionServiceResult Success(int correctAnswerId)
        {
            return new AnsweredQuestionServiceResult(correctAnswerId);
        }

        public static new AnsweredQuestionServiceResult Failed(params string[] errors)
        {
            return new AnsweredQuestionServiceResult(errors);
        }
    }
}