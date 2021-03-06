﻿using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class ProfileReviewsViewModel
    {
        public IReadOnlyCollection<ReviewViewModel> Items { get; set; }
        public string CurrentUserId { get; set; }
        public string ProfileUserId { get; set; }
        public int ProfileId { get; set; }
        public string ProfileUserName { get; set; }
    }
}