using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class LookingForLocation
    {
        public int Id { get; set; }
        public string Range { get; set; }

        public ICollection<Profile> Profiles { get; set; }
    }
}