using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class UserQuizPerformance
    {
        public int QuizId { get; set; }
        public int PointsEarned { get; set; }
        public int CorrectAnswerCount { get; set; }
    }
}