using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class GeoCity
    {
        public int Id { get; set; }

        [IndexAttribute("UX_StateAndCity", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public int GeoStateId { get; set; }

        [IndexAttribute("UX_StateAndCity", 2, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public virtual GeoState GeoState { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }
    }
}