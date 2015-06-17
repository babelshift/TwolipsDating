using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ProfileFeedViewModel
    {
        public IReadOnlyList<ProfileFeedItemViewModel> Items { get; set; }
        public string CurrentUserId { get; set; }
        public string ProfileUserId { get; set; }
        public string ProfileUserName { get; set; }
        public int ProfileId { get; set; }
    }
}