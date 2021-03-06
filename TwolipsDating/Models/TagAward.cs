﻿using System;

namespace TwolipsDating.Models
{
    public class TagAward
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int TagId { get; set; }
        public DateTime DateAwarded { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual Tag Tag { get; set; }
    }
}