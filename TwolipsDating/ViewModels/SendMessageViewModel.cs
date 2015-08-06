using System.ComponentModel.DataAnnotations;

namespace TwolipsDating.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        [Display(Name = "Message:")]
        public string MessageBody { get; set; }
    }
}