using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class GeoState
    {
        public int Id { get; set; }

        [IndexAttribute("UX_CountryAndState", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int GeoCountryId { get; set; }

        [IndexAttribute("UX_CountryAndState", 2, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Abbreviation { get; set; }

        public virtual GeoCountry GeoCountry { get; set; }

        public virtual ICollection<GeoCity> GeoCities { get; set; }
    }
}