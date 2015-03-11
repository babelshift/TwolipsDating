using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ReceivedMessageViewModel
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public int SenderProfileId { get; set; }
        public string SenderProfileImagePath { get; set; }
        public DateTime DateSent { get; set; }
        public string TimeAgo { get; set; }
        public string Body { get; set; }
    }
}