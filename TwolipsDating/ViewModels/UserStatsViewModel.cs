using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UserStatsViewModel
    {
        public int TotalPoints { get; set; }
        public int QuestionsAnswered { get; set; }
        public int QuestionsAnsweredCorrectly { get; set; }
        public int RandomQuestionsAnswered { get; set; }
        public int RandomQuestionsAnsweredCorrectly { get; set; }
        public int TimedQuestionsAnswered { get; set; }
        public int TimedQuestionsAnsweredCorrectly { get; set; }
        public int QuizQuestionsAnswered { get; set; }
        public int QuizQuestionsAnsweredCorrectly { get; set; }
        public int QuizzesCompleted { get; set; }
    }
}