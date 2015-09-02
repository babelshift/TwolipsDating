using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Profile> Profiles { get; set; }
    }
}