using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class SendGiftViewModel
    {
        [Required]
        public int GiftId { get; set; }

        [Required]
        public int InventoryItemId { get; set; }
    }
}