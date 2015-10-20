using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Business
{
    public class PossibleQuizPerformance
    {
        public int QuizId { get; set; }
        public int PointsPossible { get; set; }
        public int PossibleCorrectAnswerCount { get; set; }
        public int PointsAverage { get; set; }
    }
}