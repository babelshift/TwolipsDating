using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class SearchResultViewModel
    {
        public IReadOnlyCollection<ProfileViewModel> SearchResults { get; set; }

        public string User { get; set; }
        public string Tag { get; set; }
    }
}