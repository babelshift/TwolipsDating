using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class AnsweredQuestionResult : ServiceResult
    {
        public int CorrectAnswerId { get; private set; }

        private AnsweredQuestionResult(int correctAnswerId)
        {
            CorrectAnswerId = correctAnswerId;
        }

        private AnsweredQuestionResult(IEnumerable<string> errors)
            : base(errors) { }

        public static AnsweredQuestionResult Success(int correctAnswerId)
        {
            return new AnsweredQuestionResult(correctAnswerId);
        }

        public static AnsweredQuestionResult Failed(params string[] errors)
        {
            return new AnsweredQuestionResult(errors);
        }
    }
}