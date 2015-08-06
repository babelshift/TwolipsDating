using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public enum GenderValue
    {
        Unknown = 1,
        Male = 2,
        Woman = 3
    }

    public class Gender
    {
        public Gender()
        {
            Profiles = new List<Profile>();
        }

        public int Id { get; set; }

        [Index("UX_Name", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }
    }
}