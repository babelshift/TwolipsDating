using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ConversationViewModel
    {
        public IReadOnlyCollection<ConversationItemViewModel> Conversations { get; set; }

        public bool IsCurrentUserEmailConfirmed { get; set; }
        public int TargetProfileId { get; set; }
        public string TargetUserName { get; set; }
        public string TargetProfileImagePath { get; set;}
        public int TargetUserAge { get; set; }
        public string TargetUserLocation { get; set; }
        public IReadOnlyList<ConversationItemViewModel> ConversationMessages { get; set; }
    }
}