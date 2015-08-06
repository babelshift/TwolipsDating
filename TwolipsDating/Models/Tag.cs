﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public enum TagValues
    {
        intellectual = 1,
        neckbeard = 2,
        jock = 3,
        player = 4,
        wizard = 5,
        simple = 6,
        hot = 7,
        arrogant = 8,
        intense = 9,
        hobbit = 10,
        insecure = 11,
        bookworm = 12,
        gamer = 13,
        foody = 14,
        creative = 15,
        film_critic = 16
    }

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