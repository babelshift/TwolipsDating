using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class StoreSpotlight
    {
        public int StoreSaleId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public virtual StoreSale StoreSale { get; set; }
    }
}