﻿using System.ComponentModel.DataAnnotations;

namespace TwolipsDating.ViewModels
{
    public class HomeViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public string ReferralCode { get; set; }

        public string BackgroundImage { get; set; }
    }
}