using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class ZipCode
    {
        public string ZipCodeId { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }
    }
}