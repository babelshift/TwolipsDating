using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UserCompletedQuizViewModel
    {
        public string ProfileImagePath { get; set; }
        public string UserName { get; set; }
        public int ProfileId { get; set; }
        public string TimeAgo { get; set; }
        public int CorrectAnswerCount { get; set; }
        public int TotalAnswerCount { get; set; }
    }
}