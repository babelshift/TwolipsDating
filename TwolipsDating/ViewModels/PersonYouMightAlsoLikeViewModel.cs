using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Utilities;

namespace TwolipsDating.ViewModels
{
    public class PersonYouMightAlsoLikeViewModel
    {
        public int ProfileId { get; set; }
        public string ProfileThumbnailImagePath { get; set; }
        public string UserName { get; set; }
        public DateTime Birthday { get; set; }
        public string Age { get { return Birthday.GetAge().ToString(); } }
        public string LocationCityName { get; set; }
        public string LocationStateAbbreviation { get; set; }
        public string LocationCountryName { get; set; }
        public string Location { get { return CityExtensions.ToFullLocationString(LocationCityName, LocationStateAbbreviation, LocationCountryName); } }
        public string ProfileUserId { get; set; }
    }
}