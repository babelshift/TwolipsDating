using PagedList;
using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class ProfileFeedViewModel
    {
        public IPagedList<ProfileFeedItemViewModel> Items { get; set; }
        public string CurrentUserId { get; set; }
        public string ProfileUserId { get; set; }
        public string ProfileUserName { get; set; }
        public int ProfileId { get; set; }
    }
}