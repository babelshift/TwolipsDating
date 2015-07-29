using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class QuestionViolationType
    {
        public int Id { get; set; }

        [IndexAttribute("UX_Name", IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public virtual ICollection<QuestionViolation> QuestionViolations { get; set; }
    }
}