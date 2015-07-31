using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class UserTitle
    {
        public string UserId { get; set; }
        public int StoreItemId { get; set; }
        public DateTime DateObtained { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual StoreItem StoreItem { get; set; }
    }
}