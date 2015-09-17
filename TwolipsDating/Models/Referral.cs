using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class Referral
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public bool IsRedeemed { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}