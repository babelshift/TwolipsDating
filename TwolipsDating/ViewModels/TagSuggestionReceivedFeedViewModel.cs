﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class TagSuggestionReceivedFeedViewModel
    {
        public string SuggestUserName { get; set; }
        public string SuggestSEOName { get { return ProfileExtensions.ToSEOName(SuggestUserName); } }
        public int SuggestProfileId { get; set; }
        public string SuggestProfileImagePath { get; set; }
        public string SuggestUserId { get; set; }
        public string ReceiverUserName { get; set; }
        public string ReceiverSEOName { get { return ProfileExtensions.ToSEOName(ReceiverUserName); } }
        public int ReceiverProfileId { get; set; }
        public DateTime DateSuggested { get; set; }
        public string TimeAgo { get { return DateSuggested.GetTimeAgo(); } }
        public string TagName { get; set; }
        public IList<string> Tags { get; set; }
    }
}