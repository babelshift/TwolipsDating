using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
	public class TagSuggestion
	{
		public int ProfileId { get; set; }
		public int TagId { get; set; }
		public string SuggestingUserId { get; set; }
		public DateTime DateSuggested { get; set; }

		public virtual Profile Profile { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual ApplicationUser SuggestingUser { get; set; }
	}
}