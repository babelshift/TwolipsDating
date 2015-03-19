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

		public Profile Profile { get; set; }
		public Tag Tag { get; set; }
		public ApplicationUser SuggestingUser { get; set; }
	}
}