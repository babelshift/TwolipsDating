using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UserAnsweredQuestionCorrectlyViewModel
    {
        public string ProfileImagePath { get; set; }
        public string UserName { get; set; }
        public int ProfileId { get; set; }
        public string TimeAgo { get; set; }
    }
}