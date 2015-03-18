using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class City
    {
		public City()
		{
			Profiles = new List<Profile>();
			ZipCodes = new List<ZipCode>();
		}

        public int Id { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public int? USStateId { get; set; } // not required out of US

        public virtual Country Country { get; set; }

        public virtual USState USState { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public virtual ICollection<ZipCode> ZipCodes { get; set; }
    }
}