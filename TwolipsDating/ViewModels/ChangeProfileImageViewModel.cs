using System.ComponentModel.DataAnnotations;

namespace TwolipsDating.ViewModels
{
    public class ChangeProfileImageViewModel
    {
        [Required]
        public int UserImageId { get; set; }

        [Required]
        public int ProfileId { get; set; }
    }
}