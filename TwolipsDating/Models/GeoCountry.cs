using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class GeoCountry
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GeoState> GeoStates { get; set; }
    }
}