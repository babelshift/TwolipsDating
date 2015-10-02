using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class UserWithSimilarQuizScoreViewModel
    {
        public string UserName { get; set; }
        public int ProfileId { get; set; }
        public string ProfileThumbnailImagePath { get; set; }
        public double Score { get; set; }
        public int PercentScore { get { return (int)Math.Round(Score * 100); } }
    }
}