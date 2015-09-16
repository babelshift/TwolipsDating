using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UserToMessageViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProfileThumbnailImagePath { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
    }
}