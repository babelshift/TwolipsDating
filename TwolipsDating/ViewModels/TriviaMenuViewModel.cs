﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class TriviaMenuViewModel
    {
        public IReadOnlyCollection<QuizOverviewViewModel> Quizzes { get; set; }
        public UserStatsViewModel UserStats { get; set; }
    }
}