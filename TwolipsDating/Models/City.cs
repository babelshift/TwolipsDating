using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public int? USStateId { get; set; } // not required out of US

        public virtual Country Country { get; set; }

        public virtual USState USState { get; set; }

        public ICollection<Profile> Profiles { get; set; }

        public ICollection<ZipCode> ZipCodes { get; set; }
    }
}