using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class MessageViewModel
    {
        public IReadOnlyCollection<ReceivedMessageViewModel> ReceivedMessages { get; set; }
        public IReadOnlyCollection<SentMessageViewModel> SentMessages { get; set; }
    }
}