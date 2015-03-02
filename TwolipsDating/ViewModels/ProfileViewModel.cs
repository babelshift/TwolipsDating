using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage="Month must be between 1 and 12")]
        public int? BirthMonth { get; set; }

        [Required]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
        public int? BirthDayOfMonth { get; set; }

        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 18 years ago")]
        public int? BirthYear { get; set; }
        
        [Required]
        public int? SelectedGenderId { get; set; }
        
        [Required]
        public int? SelectedCountryId { get; set; }

        public int? SelectedZipCodeId { get; set; }
        public int? SelectedCityId { get; set; }

        public IDictionary<int, string> Genders { get; set; }
        public IDictionary<int, string> Countries { get; set; }
    }
}