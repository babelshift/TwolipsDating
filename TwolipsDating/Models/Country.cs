﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class Country
    {
        public Country()
        {
            Cities = new List<City>();
        }

        public int Id { get; set; }

        [Index("UX_Name", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}