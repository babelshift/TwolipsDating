using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ProfileTagSuggestionViewModel
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public int TagCount { get; set; }
        public bool DidUserSuggest { get; set; }
    }
}