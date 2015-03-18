﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class Profile
    {
		public Profile()
		{
			Tags = new List<Tag>();
		}

        public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public int GenderId { get; set; }
        public int? ZipCode { get; set; } // not required out of US
        public int CityId { get; set; }
        public int? UserImageId { get; set; }

        public virtual UserImage UserImage { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual City City { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

		public virtual ICollection<Tag> Tags { get; set; }
    }
}