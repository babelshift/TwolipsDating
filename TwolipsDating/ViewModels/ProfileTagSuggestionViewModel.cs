using System;
using System.Collections.Generic;
namespace TwolipsDating.ViewModels
{
    public class ProfileTagSuggestionViewModel
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }
        public int TagCount { get; set; }
        public bool DidUserSuggest { get; set; }

        public IReadOnlyCollection<TagSuggestionUserViewModel> SuggestionUsers { get; set; }
    }
}