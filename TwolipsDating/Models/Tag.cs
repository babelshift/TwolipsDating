using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
	public class Tag
	{
		public Tag()
		{
			Profiles = new List<Profile>();
		}

		public int TagId { get; set; }

		[Index("UX_Name", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
		public string Name { get; set; }
		public string Description { get; set; }

		public virtual ICollection<Profile> Profiles { get; set; }

        public virtual ICollection<TagSuggestion> TagSuggestions { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<TagAward> TagAwards { get; set; }
	}
}