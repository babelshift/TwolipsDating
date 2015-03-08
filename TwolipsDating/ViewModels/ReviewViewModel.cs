using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ReviewViewModel
    {
        public string AuthorUserName { get; set; }
        public string TimeAgo { get; set; }
        public int RatingValue { get; set; }
        public string Content { get; set; }
        public string ProfileImagePath { get; set; }
    }
}