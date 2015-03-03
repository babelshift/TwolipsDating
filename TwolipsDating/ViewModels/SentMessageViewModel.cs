using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class SentMessageViewModel
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; }
        public DateTime DateSent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}