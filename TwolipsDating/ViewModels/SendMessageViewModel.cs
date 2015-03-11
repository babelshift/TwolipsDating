using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        [Display(Name = "Message:")]
        public string MessageBody { get; set; }

    }
}