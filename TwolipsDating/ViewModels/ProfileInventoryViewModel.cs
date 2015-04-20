using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.ViewModels
{
    public class ProfileInventoryViewModel
    {
        public IReadOnlyCollection<InventoryItemViewModel> Items { get; set; }
        public string CurrentUserId { get; set; }
        public string ProfileUserId { get; set; }
    }
}