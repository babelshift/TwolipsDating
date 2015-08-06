﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class ViolationType
    {
        public int Id { get; set; }

        [IndexAttribute("UX_Name", IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public virtual ICollection<ReviewViolation> ReviewViolations { get; set; }
    }
}