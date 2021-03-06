﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class RecentBuyerViewModel
    {
        public string ProfileImagePath { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int ProfileId { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Location { get; set; }
    }
}