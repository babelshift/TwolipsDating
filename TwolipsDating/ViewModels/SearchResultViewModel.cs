using PagedList;
using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class SearchResultViewModel
    {
        public IPagedList<ProfileViewModel> SearchResults { get; set; }
        public List<string> Tags { get; set; }
        public IDictionary<string, string> SearchTags { get; set; }
    }
}